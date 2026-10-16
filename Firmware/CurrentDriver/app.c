#include "hwbp_core.h"
#include "hwbp_core_regs.h"
#include "hwbp_core_types.h"

#include "app.h"
#include "app_funcs.h"
#include "app_ios_and_regs.h"

#include "structs.h"
#include <util/atomic.h>

#define F_CPU 32000000 //need to be defined for delay.h
#include <util/delay.h>

/************************************************************************/
/* Declare application registers                                        */
/************************************************************************/
extern AppRegs app_regs;
extern uint8_t app_regs_type[];
extern uint16_t app_regs_n_elements[];
extern uint8_t *app_regs_pointer[];
extern void (*app_func_rd_pointer[])(void);
extern bool (*app_func_wr_pointer[])(void*);

/************************************************************************/
/* Initialize app                                                       */
/************************************************************************/
static const uint8_t default_device_name[] = "CurrentDriver";

void hwbp_app_initialize(void)
{
    /* Define versions */
    uint8_t hwH = 1;
    uint8_t hwL = 1;
    uint8_t fwH = 1;
    uint8_t fwL = 0;
    uint8_t ass = 0;
    
   	/* Start core */
    core_func_start_core(
        1282,
        hwH, hwL,
        fwH, fwL,
        ass,
        (uint8_t*)(&app_regs),
        APP_NBYTES_OF_REG_BANK,
        APP_REGS_ADD_MAX - APP_REGS_ADD_MIN + 1,
        default_device_name,
        false,	// The device is _not_ able to repeat the harp timestamp clock
        false,	// The device is _not_ able to generate the harp timestamp clock
        0		// Default timestamp offset										 								   
    );
}

/************************************************************************/
/* Handle if a catastrophic error occur                                 */
/************************************************************************/
void core_callback_catastrophic_error_detected(void)
{
	set_I_OFF_DAC0;
	set_I_OFF_DAC1;
	clr_DO1;
	clr_DO0;
	
}

/************************************************************************/
/* General definitions                                                  */
/************************************************************************/

/* Runtime state is private; all entry points below run with interrupts masked. */
static volatile ChannelState channels[2];

static uint8_t channel_mode(uint8_t channel)
{
    return channel ? app_regs.REG_CHANNEL1_MODE : app_regs.REG_CHANNEL0_MODE;
}

static void output_write(uint8_t channel, uint16_t value)
{
    if (channel)
        latch_dac1(value);
    else
        latch_dac0(value);
    channels[channel].output.applied_value = value;
}

static uint16_t configured_target(uint8_t channel)
{
    float voltage = channel ? app_regs.REG_DAC1_VOLTAGE : app_regs.REG_DAC0_VOLTAGE;
    return (uint16_t)(voltage / 5000.0f * 65535.0f);
}

static void output_configure(uint8_t channel, bool preserve_phase)
{
    volatile OutputState *output = &channels[channel].output;
    bool pulses = (app_regs.REG_PULSE_ENABLE & (channel ? B_LED1 : B_LED0)) != 0;

    if (pulses) {
        uint8_t frequency = channel ? app_regs.REG_PULSE_FREQUENCY_LED1 : app_regs.REG_PULSE_FREQUENCY_LED0;
        uint8_t duty = channel ? app_regs.REG_PULSE_DCYCLE_LED1 : app_regs.REG_PULSE_DCYCLE_LED0;
        uint16_t period = (2000U + frequency / 2U) / frequency;
        uint16_t on_ticks = ((uint32_t)period * duty + 50U) / 100U;
        bool keep_phase = preserve_phase && output->kind == WAVEFORM_PULSE &&
            output->waveform.pulse.off_ticks != 0;

        if (on_ticks == 0)
            on_ticks = 1;
        if (duty < 100 && on_ticks >= period)
            on_ticks = period - 1;
        if (!keep_phase || duty == 100) {
            output->waveform.pulse.on = true;
            output->waveform.pulse.remaining_ticks = on_ticks;
        }
        output->kind = WAVEFORM_PULSE;
        output->waveform.pulse.on_ticks = on_ticks;
        output->waveform.pulse.off_ticks = period - on_ticks;
    } else {
        output->kind = WAVEFORM_LEVEL;
        output->waveform.ramp.rise_enabled = (app_regs.REG_RAMP_CONFIG & (channel ? B_LED1_RISE : B_LED0_RISE)) != 0;
        output->waveform.ramp.fall_enabled = (app_regs.REG_RAMP_CONFIG & (channel ? B_LED1_FALL : B_LED0_FALL)) != 0;
        output->waveform.ramp.duration_ms = channel ? app_regs.REG_RAMP_LED1 : app_regs.REG_RAMP_LED0;
        output->waveform.ramp.remaining_ticks = 0;
    }
}

static void output_begin(uint8_t channel, uint16_t target)
{
    volatile OutputState *output = &channels[channel].output;
    output->target = target;
    if (output->kind == WAVEFORM_PULSE) {
        output_write(channel, output->waveform.pulse.on ? target : 0);
    } else {
        volatile RampState *ramp = &output->waveform.ramp;
        bool rising = target > output->applied_value;
        bool enabled = rising ? ramp->rise_enabled : ramp->fall_enabled;
        uint16_t delta = rising ? target - output->applied_value : output->applied_value - target;

        ramp->remaining_ticks = 0;
        if (delta && enabled && ramp->duration_ms) {
            ramp->rising = rising;
            ramp->total_ticks = (uint32_t)ramp->duration_ms * 2U;
            ramp->remaining_ticks = ramp->total_ticks;
            ramp->step = delta / ramp->total_ticks;
            ramp->remainder = delta % ramp->total_ticks;
            ramp->error = 0;
        } else {
            output_write(channel, target);
        }
    }
}

static bool output_settled(uint8_t channel)
{
    return channels[channel].output.kind == WAVEFORM_PULSE ||
        channels[channel].output.waveform.ramp.remaining_ticks == 0;
}

static void output_tick(uint8_t channel)
{
    volatile OutputState *output = &channels[channel].output;
    if (output->kind == WAVEFORM_PULSE) {
        volatile PulseState *pulse = &output->waveform.pulse;
        /* A 100% duty cycle has no OFF phase. */
        if (pulse->off_ticks == 0)
            return;
        if (pulse->remaining_ticks && --pulse->remaining_ticks == 0) {
            pulse->on = !pulse->on;
            pulse->remaining_ticks = pulse->on ? pulse->on_ticks : pulse->off_ticks;
            output_write(channel, pulse->on ? output->target : 0);
        }
    } else {
        volatile RampState *ramp = &output->waveform.ramp;
        if (ramp->remaining_ticks) {
            uint16_t step = ramp->step;
            ramp->error += ramp->remainder;
            if (ramp->error >= ramp->total_ticks) {
                ramp->error -= ramp->total_ticks;
                step++;
            }
            --ramp->remaining_ticks;
            output_write(channel, ramp->remaining_ticks == 0 ? output->target :
                (ramp->rising ? output->applied_value + step : output->applied_value - step));
        }
    }
}

static void protocol_stop(uint8_t channel)
{
    volatile OutputState *output = &channels[channel].output;
    if (channels[channel].protocol.phase == PROTOCOL_IDLE)
        return;
    if (channels[channel].protocol.phase == PROTOCOL_DELAY || output->kind == WAVEFORM_PULSE) {
        output->kind = WAVEFORM_LEVEL;
        output->waveform.ramp.remaining_ticks = 0;
        output->target = 0;
        output_write(channel, 0);
    } else {
        output_begin(channel, 0);
    }
    channels[channel].protocol.phase = output_settled(channel) ? PROTOCOL_IDLE : PROTOCOL_STOPPING;
}

void channel_reset(uint8_t channel)
{
    ChannelState reset = {0};
    ATOMIC_BLOCK(ATOMIC_RESTORESTATE) {
        channels[channel] = reset;
        output_write(channel, 0);
    }
}

void channel_apply_manual(uint8_t channel)
{
    ATOMIC_BLOCK(ATOMIC_RESTORESTATE) {
        channels[channel].protocol.phase = PROTOCOL_IDLE;
        output_configure(channel, true);
        output_begin(channel, configured_target(channel));
    }
}

void channel_start_protocol(uint8_t channel)
{
    ATOMIC_BLOCK(ATOMIC_RESTORESTATE) {
        volatile ProtocolState *protocol = &channels[channel].protocol;
        /* Retriggering starts a fresh execution, including a new delay. */
        output_write(channel, 0);
        output_configure(channel, false);
        channels[channel].output.target = configured_target(channel);
        protocol->delay_ticks = (uint32_t)(channel ? app_regs.REG_PROTOCOL1_DELAY : app_regs.REG_PROTOCOL0_DELAY) * 2U;
        protocol->duration_ticks = (uint32_t)(channel ? app_regs.REG_PROTOCOL1_DURATION : app_regs.REG_PROTOCOL0_DURATION) * 2U;
        protocol->has_duration = channel_mode(channel) != GM_START_AND_STOP_PROTOCOL_TRIGGER && protocol->duration_ticks != 0;
        if (protocol->delay_ticks) {
            protocol->phase = PROTOCOL_DELAY;
        } else {
            output_begin(channel, channels[channel].output.target);
            protocol->phase = output_settled(channel) ? PROTOCOL_HOLD : PROTOCOL_STARTING;
        }
    }
}

void channel_stop_protocol(uint8_t channel)
{
    ATOMIC_BLOCK(ATOMIC_RESTORESTATE) {
        protocol_stop(channel);
    }
}

bool channel_set_mode(uint8_t channel, uint8_t mode)
{
    if (mode > GM_START_AND_STOP_PROTOCOL_TRIGGER)
        return false;
    ATOMIC_BLOCK(ATOMIC_RESTORESTATE) {
        if (mode != channel_mode(channel)) {
            channel_reset(channel);
            if (channel)
                app_regs.REG_CHANNEL1_MODE = mode;
            else
                app_regs.REG_CHANNEL0_MODE = mode;
            if (mode == GM_MANUAL)
                channel_apply_manual(channel);
        }
    }
    return true;
}

void channels_tick_500us(void)
{
    uint8_t channel;
    ATOMIC_BLOCK(ATOMIC_RESTORESTATE) {
        for (channel = 0; channel < 2; channel++) {
            volatile ProtocolState *protocol = &channels[channel].protocol;
            if (protocol->phase == PROTOCOL_DELAY) {
                if (--protocol->delay_ticks == 0) {
                    output_begin(channel, channels[channel].output.target);
                    protocol->phase = output_settled(channel) ? PROTOCOL_HOLD : PROTOCOL_STARTING;
                }
                continue;
            }
            if (protocol->phase == PROTOCOL_HOLD && protocol->has_duration) {
                if (--protocol->duration_ticks == 0) {
                    protocol_stop(channel);
                    continue;
                }
            }
            output_tick(channel);
            if (protocol->phase == PROTOCOL_STARTING && output_settled(channel))
                protocol->phase = PROTOCOL_HOLD;
            else if (protocol->phase == PROTOCOL_STOPPING && output_settled(channel))
                protocol->phase = PROTOCOL_IDLE;
        }
    }
}


/************************************************************************/
/* User functions                                                       */
/************************************************************************/
/* Add your functions here or load external functions if needed */

/************************************************************************/
/* Initialization Callbacks                                             */
/************************************************************************/
void core_callback_define_clock_default(void) {}

void core_callback_initialize_hardware(void){

	init_ios();

	/* Initialize SPI with 4MHz */
	SPID_CTRL = SPI_MASTER_bm | SPI_ENABLE_bm | SPI_MODE_0_gc | SPI_CLK2X_bm | SPI_PRESCALER_DIV16_gc;

}

void core_callback_1st_config_hw_after_boot(void)
{
	/* Initialize IOs */
	/* Don't delete this function!!! */
	init_ios();
	
	/* Initialize SPI with 4MHz */
	SPID_CTRL = SPI_MASTER_bm | SPI_ENABLE_bm | SPI_MODE_0_gc | SPI_CLK2X_bm | SPI_PRESCALER_DIV16_gc;

}

void core_callback_reset_registers(void)
{
	/* Initialize registers */
	app_regs.REG_PORT_DIS = 0;
	app_regs.REG_OUTPUTS_SET = 0;
	app_regs.REG_OUTPUTS_CLEAR = 0;
	app_regs.REG_OUTPUTS_TOGGLE = 0;
	app_regs.REG_OUTPUTS_OUT = 0;
	app_regs.REG_LED_ENABLE = 0;
	app_regs.REG_LED_DISABLE = 0;
	app_regs.REG_LED_OUT = 0;
	app_regs.REG_LED0_CURRENT = 0;
	app_regs.REG_LED1_CURRENT = 0;
	app_regs.REG_LED0_MAX_CURRENT = 100;
	app_regs.REG_LED1_MAX_CURRENT = 100;
	app_regs.REG_DAC0_VOLTAGE = 0;
	app_regs.REG_DAC1_VOLTAGE = 0;
	app_regs.REG_PULSE_ENABLE = 0;
	app_regs.REG_PULSE_DCYCLE_LED0 = 1;
	app_regs.REG_PULSE_DCYCLE_LED1 = 1;
	app_regs.REG_PULSE_FREQUENCY_LED0 = 1;
	app_regs.REG_PULSE_FREQUENCY_LED1 = 1;
	app_regs.REG_RAMP_LED0 = 1;
	app_regs.REG_RAMP_LED1 = 1;
	app_regs.REG_RAMP_CONFIG = 0;
	app_regs.REG_PROTOCOL0_DURATION = 0;
	app_regs.REG_PROTOCOL1_DURATION = 0;
	app_regs.REG_PROTOCOL0_DELAY = 0;
	app_regs.REG_PROTOCOL1_DELAY = 0;
	app_regs.REG_ENABLE_PROTOCOL = 0;
	app_regs.REG_DISABLE_PROTOCOL = 0;
	app_regs.REG_CHANNEL0_MODE = 0;
	app_regs.REG_CHANNEL1_MODE = 0;
	app_regs.REG_CHANNEL_STATE = 0;
	app_regs.REG_EVNT_ENABLE = B_EVT_PORT_DIS | B_EVT_CHANNEL_STATE;
	
}

void core_callback_registers_were_reinitialized(void)
{
	channel_reset(0);
	channel_reset(1);
	
}

/************************************************************************/
/* Callbacks: Visualization                                             */
/************************************************************************/
void core_callback_visualen_to_on(void)
{
	/* Update visual indicators */
	
}

void core_callback_visualen_to_off(void)
{
	/* Clear all the enabled indicators */
	
}

/************************************************************************/
/* Callbacks: Change on the operation mode                              */
/************************************************************************/
void core_callback_device_to_standby(void) {}
void core_callback_device_to_active(void) {}
void core_callback_device_to_enchanced_active(void) {}
void core_callback_device_to_speed(void) {}

/************************************************************************/
/* Callbacks: 1 ms timer                                                */
/************************************************************************/
void core_callback_t_before_exec(void) {}
void core_callback_t_after_exec(void) {}
void core_callback_t_new_second(void) {}
void core_callback_t_500us(void)
{
    channels_tick_500us();
}
void core_callback_t_1ms(void) {}


/************************************************************************/
/* Callbacks: clock control                                              */
/************************************************************************/
void core_callback_clock_to_repeater(void) {}
void core_callback_clock_to_generator(void) {}
void core_callback_clock_to_unlock(void) {}
void core_callback_clock_to_lock(void) {}
	
/************************************************************************/																  
/* Callbacks: uart control                                              */
/************************************************************************/
void core_callback_uart_rx_before_exec(void) {}
void core_callback_uart_rx_after_exec(void) {}
void core_callback_uart_tx_before_exec(void) {}
void core_callback_uart_tx_after_exec(void) {}
void core_callback_uart_cts_before_exec(void) {}
void core_callback_uart_cts_after_exec(void) {}

/************************************************************************/
/* Callbacks: Read app register                                         */
/************************************************************************/
bool core_read_app_register(uint8_t add, uint8_t type)
{
	/* Check if it will not access forbidden memory */
	if (add < APP_REGS_ADD_MIN || add > APP_REGS_ADD_MAX)
		return false;
	
	/* Check if type matches */
	if (app_regs_type[add-APP_REGS_ADD_MIN] != type)
		return false;
	
	/* Receive data */
	(*app_func_rd_pointer[add-APP_REGS_ADD_MIN])();	

	/* Return success */
	return true;
}

/************************************************************************/
/* Callbacks: Write app register                                        */
/************************************************************************/
bool core_write_app_register(uint8_t add, uint8_t type, uint8_t * content, uint16_t n_elements)
{
	/* Check if it will not access forbidden memory */
	if (add < APP_REGS_ADD_MIN || add > APP_REGS_ADD_MAX)
		return false;
	
	/* Check if type matches */
	if (app_regs_type[add-APP_REGS_ADD_MIN] != type)
		return false;

	/* Check if the number of elements matches */
	if (app_regs_n_elements[add-APP_REGS_ADD_MIN] != n_elements)
		return false;

	/* Process data and return false if write is not allowed or contains errors */
	bool result;
    ATOMIC_BLOCK(ATOMIC_RESTORESTATE) {
        result = (*app_func_wr_pointer[add-APP_REGS_ADD_MIN])(content);
    }
    return result;
}
