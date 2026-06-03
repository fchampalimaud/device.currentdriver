#include "app_funcs.h"
#include "app_ios_and_regs.h"
#include "hwbp_core.h"

#define F_CPU 32000000
#include <util/delay.h>

#include "structs.h"

extern Protocol protocol0;
extern Protocol protocol1;

/************************************************************************/
/* Create pointers to functions                                         */
/************************************************************************/
extern AppRegs app_regs;

void (*app_func_rd_pointer[])(void) = {
	&app_read_REG_PORT_DIS,
	&app_read_REG_OUTPUTS_SET,
	&app_read_REG_OUTPUTS_CLEAR,
	&app_read_REG_OUTPUTS_TOGGLE,
	&app_read_REG_OUTPUTS_OUT,
	&app_read_REG_LED_ENABLE,
	&app_read_REG_LED_DISABLE,
	&app_read_REG_LED_OUT,
	&app_read_REG_LED_TARGET_STATE,
	&app_read_REG_LED0_CURRENT,
	&app_read_REG_LED1_CURRENT,
	&app_read_REG_LED0_MAX_CURRENT,
	&app_read_REG_LED1_MAX_CURRENT,
	&app_read_REG_DAC0_VOLTAGE,
	&app_read_REG_DAC1_VOLTAGE,
	&app_read_REG_PULSE_ENABLE,
	&app_read_REG_PULSE_DCYCLE_LED0,
	&app_read_REG_PULSE_DCYCLE_LED1,
	&app_read_REG_PULSE_FREQUENCY_LED0,
	&app_read_REG_PULSE_FREQUENCY_LED1,
	&app_read_REG_RAMP_LED0,
	&app_read_REG_RAMP_LED1,
	&app_read_REG_RAMP_CONFIG,
	&app_read_REG_PROTOCOL0_DURATION,
	&app_read_REG_PROTOCOL1_DURATION,
	&app_read_REG_PROTOCOL0_DELAY,
	&app_read_REG_PROTOCOL1_DELAY,
	&app_read_REG_ENABLE_PROTOCOL,
	&app_read_REG_DISABLE_PROTOCOL,
	&app_read_REG_DI0_TRIGGER,
	&app_read_REG_DI1_TRIGGER,
	&app_read_REG_RESERVED0,
	&app_read_REG_RESERVED1,
	&app_read_REG_RESERVED2,
	&app_read_REG_RESERVED3,
	&app_read_REG_EVNT_ENABLE
};

bool (*app_func_wr_pointer[])(void*) = {
	&app_write_REG_PORT_DIS,
	&app_write_REG_OUTPUTS_SET,
	&app_write_REG_OUTPUTS_CLEAR,
	&app_write_REG_OUTPUTS_TOGGLE,
	&app_write_REG_OUTPUTS_OUT,
	&app_write_REG_LED_ENABLE,
	&app_write_REG_LED_DISABLE,
	&app_write_REG_LED_OUT,
	&app_write_REG_LED_TARGET_STATE,
	&app_write_REG_LED0_CURRENT,
	&app_write_REG_LED1_CURRENT,
	&app_write_REG_LED0_MAX_CURRENT,
	&app_write_REG_LED1_MAX_CURRENT,
	&app_write_REG_DAC0_VOLTAGE,
	&app_write_REG_DAC1_VOLTAGE,
	&app_write_REG_PULSE_ENABLE,
	&app_write_REG_PULSE_DCYCLE_LED0,
	&app_write_REG_PULSE_DCYCLE_LED1,
	&app_write_REG_PULSE_FREQUENCY_LED0,
	&app_write_REG_PULSE_FREQUENCY_LED1,
	&app_write_REG_RAMP_LED0,
	&app_write_REG_RAMP_LED1,
	&app_write_REG_RAMP_CONFIG,
	&app_write_REG_PROTOCOL0_DURATION,
	&app_write_REG_PROTOCOL1_DURATION,
	&app_write_REG_PROTOCOL0_DELAY,
	&app_write_REG_PROTOCOL1_DELAY,
	&app_write_REG_ENABLE_PROTOCOL,
	&app_write_REG_DISABLE_PROTOCOL,
	&app_write_REG_DI0_TRIGGER,
	&app_write_REG_DI1_TRIGGER,
	&app_write_REG_RESERVED0,
	&app_write_REG_RESERVED1,
	&app_write_REG_RESERVED2,
	&app_write_REG_RESERVED3,
	&app_write_REG_EVNT_ENABLE
};


/************************************************************************/
/* REG_PORT_DIS                                                         */
/************************************************************************/
void app_read_REG_PORT_DIS(void)
{
	app_regs.REG_PORT_DIS = (read_DI0) ? B_DI0 : 0;
	app_regs.REG_PORT_DIS |= (read_DI1) ? B_DI1 : 0;
}

bool app_write_REG_PORT_DIS(void *a) { return false; }


/************************************************************************/
/* REG_OUTPUTS_SET                                                      */
/************************************************************************/
void app_read_REG_OUTPUTS_SET(void) {}
bool app_write_REG_OUTPUTS_SET(void *a)
{
	uint16_t reg = *((uint16_t*)a);

	if (reg & B_DO0) set_DO0;
	if (reg & B_DO1) set_DO1;
	
	app_regs.REG_OUTPUTS_OUT |= reg;
	app_regs.REG_OUTPUTS_SET = reg;

	return true;
}


/************************************************************************/
/* REG_OUTPUTS_CLEAR                                                    */
/************************************************************************/
void app_read_REG_OUTPUTS_CLEAR(void) {}
bool app_write_REG_OUTPUTS_CLEAR(void *a)
{
	uint16_t reg = *((uint16_t*)a);

	if (reg & B_DO0) clr_DO0;
	if (reg & B_DO1) clr_DO1;

	app_regs.REG_OUTPUTS_OUT &= ~reg;
	app_regs.REG_OUTPUTS_CLEAR = reg;
	
	return true;
}


/************************************************************************/
/* REG_OUTPUTS_TOGGLE                                                   */
/************************************************************************/
void app_read_REG_OUTPUTS_TOGGLE(void) {}
bool app_write_REG_OUTPUTS_TOGGLE(void *a)
{
	uint16_t reg = *((uint16_t*)a);
	
	if (reg & B_DO0) { tgl_DO0; }
	if (reg & B_DO1) { tgl_DO1; }

	app_regs.REG_OUTPUTS_OUT ^= reg;
	app_regs.REG_OUTPUTS_TOGGLE = reg;
	
	return true;
}


/************************************************************************/
/* REG_OUTPUTS_OUT                                                      */
/************************************************************************/
void app_read_REG_OUTPUTS_OUT(void)
{
	app_regs.REG_OUTPUTS_OUT = (read_DO0) ? B_DO0 : 0;
	app_regs.REG_OUTPUTS_OUT |= (read_DO1) ? B_DO1 : 0;
}

bool app_write_REG_OUTPUTS_OUT(void *a)
{
	uint16_t reg = *((uint16_t*)a);

	if (reg & B_DO0) set_DO0; else clr_DO0;
	if (reg & B_DO1) set_DO1; else clr_DO1;

	app_regs.REG_OUTPUTS_OUT = reg;

	return true;
}


/************************************************************************/
/* LATCH_DAC0                                                           */
/************************************************************************/
uint16_t aux_u16b;
void latch_dac0(uint16_t word)
{
	clr_CS0;
	aux_u16b = word;
	
	// 16 bit code latch
	SPID_DATA = *(((uint8_t*)(&aux_u16b))+1);
	loop_until_bit_is_set(SPID_STATUS, SPI_IF_bp);

	SPID_DATA = *(((uint8_t*)(&aux_u16b))+0);
	loop_until_bit_is_set(SPID_STATUS, SPI_IF_bp);

	set_CS0;
}


/************************************************************************/
/* LATCH_DAC1                                                           */
/************************************************************************/
void latch_dac1(uint16_t word)
{
	clr_CS1;
	aux_u16b = word;
	
	// 16 bit code latch
	SPID_DATA = *(((uint8_t*)(&aux_u16b))+1);
	loop_until_bit_is_set(SPID_STATUS, SPI_IF_bp);

	SPID_DATA = *(((uint8_t*)(&aux_u16b))+0);
	loop_until_bit_is_set(SPID_STATUS, SPI_IF_bp);

	set_CS1;
}


/************************************************************************/
/* REG_LED_ENABLE                                                       */
/************************************************************************/
void app_read_REG_LED_ENABLE(void) {}
bool app_write_REG_LED_ENABLE(void *a)
{
	uint8_t reg = *((uint8_t*)a);

	if (reg & B_LED0) 
	{
		if (app_regs.REG_LED0_CURRENT > app_regs.REG_LED0_MAX_CURRENT)
		{
			app_regs.REG_LED0_CURRENT = app_regs.REG_LED0_MAX_CURRENT;
			app_regs.REG_DAC0_VOLTAGE = 5 * app_regs.REG_LED0_MAX_CURRENT;
		}
		clr_I_OFF_DAC0;
	}

	if (reg & B_LED1) 
	{
		if (app_regs.REG_LED1_CURRENT > app_regs.REG_LED1_MAX_CURRENT)
		{
			app_regs.REG_LED1_CURRENT = app_regs.REG_LED1_MAX_CURRENT;
			app_regs.REG_DAC1_VOLTAGE = 5 * app_regs.REG_LED1_MAX_CURRENT;
		}
		clr_I_OFF_DAC1;
	}

	app_regs.REG_LED_OUT &= ~reg;
	app_regs.REG_LED_ENABLE = reg;
	
	return true;
}


/************************************************************************/
/* REG_LED_DISABLE                                                      */
/************************************************************************/
void app_read_REG_LED_DISABLE(void) {}
bool app_write_REG_LED_DISABLE(void *a)
{
	uint8_t reg = *((uint8_t*)a);

	if (reg & B_LED0) set_I_OFF_DAC0;
	if (reg & B_LED1) set_I_OFF_DAC1;

	app_regs.REG_LED_OUT |= reg;
	app_regs.REG_LED_DISABLE = reg;

	return true;
}


/************************************************************************/
/* REG_LED_OUT                                                          */
/************************************************************************/
void app_read_REG_LED_OUT(void) {
	app_regs.REG_OUTPUTS_OUT = (read_I_OFF_DAC0) ? B_LED0 : 0;
	app_regs.REG_OUTPUTS_OUT |= (read_I_OFF_DAC1) ? B_LED1 : 0;
}

bool app_write_REG_LED_OUT(void *a)
{
	uint8_t reg = *((uint8_t*)a);

	if (reg & B_LED0) 
	{
		if (app_regs.REG_LED0_CURRENT > app_regs.REG_LED0_MAX_CURRENT)
		{
			app_regs.REG_LED0_CURRENT = app_regs.REG_LED0_MAX_CURRENT;
			app_regs.REG_DAC0_VOLTAGE = 5 * app_regs.REG_LED0_MAX_CURRENT;
		}
		clr_I_OFF_DAC0;
	} else {
		set_I_OFF_DAC0;
	}

	if (reg & B_LED1) {
		if (app_regs.REG_LED1_CURRENT > app_regs.REG_LED1_MAX_CURRENT)
		{
			app_regs.REG_LED1_CURRENT = app_regs.REG_LED1_MAX_CURRENT;
			app_regs.REG_DAC1_VOLTAGE = 5 * app_regs.REG_LED1_MAX_CURRENT;
		}
		clr_I_OFF_DAC1;
	} else {
		set_I_OFF_DAC1;
	}

	app_regs.REG_LED_OUT = reg;

	return true;
}


/************************************************************************/
/* REG_LED_TARGET_STATE                                                 */
/************************************************************************/
// TODO
void app_read_REG_LED_TARGET_STATE(void) {}
bool app_write_REG_LED_TARGET_STATE(void *a) { return false; }


/************************************************************************/
/* REG_LED0_CURRENT                                                     */
/************************************************************************/
void app_read_REG_LED0_CURRENT(void) {}
bool app_write_REG_LED0_CURRENT(void *a)
{
	float reg = *((float*)a);
	
	if (reg < 0 || (reg > 1000 && reg > app_regs.REG_LED0_MAX_CURRENT))
	{
		return false;
	}

	app_regs.REG_LED0_CURRENT = reg;
	app_regs.REG_DAC0_VOLTAGE = 5 * reg;

	return true;
}


/************************************************************************/
/* REG_LED1_CURRENT                                                     */
/************************************************************************/
void app_read_REG_LED1_CURRENT(void) {}
bool app_write_REG_LED1_CURRENT(void *a)
{
	float reg = *((float*)a);

	if (reg < 0 || (reg > 1000 && reg > app_regs.REG_LED1_MAX_CURRENT))
	{
		return false;
	}

	app_regs.REG_LED1_CURRENT = reg;
	app_regs.REG_DAC1_VOLTAGE = 5 * reg;

	return true;
}


/************************************************************************/
/* REG_LED0_MAX_CURRENT                                                 */
/************************************************************************/
void app_read_REG_LED0_MAX_CURRENT(void) {}
bool app_write_REG_LED0_MAX_CURRENT(void *a)
{
	float reg = *((float*)a);

	if (reg < 1 || reg > 1000)
	{
		return false;
	}

	if (app_regs.REG_LED0_CURRENT > reg && app_regs.REG_LED_OUT & B_LED0)
	{
		app_regs.REG_LED0_CURRENT = reg;
		app_regs.REG_DAC0_VOLTAGE = 5 * reg;
	}
	
	app_regs.REG_LED0_MAX_CURRENT = reg;
	
	return true;
}


/************************************************************************/
/* REG_LED1_MAX_CURRENT                                                 */
/************************************************************************/
void app_read_REG_LED1_MAX_CURRENT(void) {}
bool app_write_REG_LED1_MAX_CURRENT(void *a)
{
	float reg = *((float*)a);
	
	if (reg < 1 || reg > 1000)
	{
		return false;
	}

	if (app_regs.REG_LED1_CURRENT > reg && app_regs.REG_LED_OUT & B_LED1)
	{
		app_regs.REG_LED1_CURRENT = reg;
		app_regs.REG_DAC1_VOLTAGE = 5 * reg;
	}

	app_regs.REG_LED1_MAX_CURRENT = reg;
	
	return true;
}


/************************************************************************/
/* REG_DAC0_VOLTAGE                                                     */
/************************************************************************/
void app_read_REG_DAC0_VOLTAGE(void) {}
bool app_write_REG_DAC0_VOLTAGE(void *a)
{
	float reg = *((float*)a);

	if (reg < 0 || reg > 5000 || ((reg / 5) > app_regs.REG_LED0_MAX_CURRENT && app_regs.REG_LED_OUT & B_LED0))
	{
		return false;
	}

	app_regs.REG_LED0_CURRENT = reg / 5;
	app_regs.REG_DAC0_VOLTAGE = reg;

	return true;
}


/************************************************************************/
/* REG_DAC1_VOLTAGE                                                     */
/************************************************************************/
void app_read_REG_DAC1_VOLTAGE(void) {}
bool app_write_REG_DAC1_VOLTAGE(void *a)
{
	float reg = *((float*)a);

	if (reg < 0 || reg > 5000)
	{
		return false;
	}

	if (reg < 0 || reg > 5000 || ((reg / 5) > app_regs.REG_LED1_MAX_CURRENT && app_regs.REG_LED_OUT & B_LED1))
	{
		return false;
	}

	app_regs.REG_LED1_CURRENT = reg / 5;
	app_regs.REG_DAC1_VOLTAGE = reg;

	return true;
}


/************************************************************************/
/* REG_PULSE_ENABLE                                                     */
/************************************************************************/
void app_read_REG_PULSE_ENABLE(void) {}
bool app_write_REG_PULSE_ENABLE(void *a)
{
	uint8_t reg = *((uint8_t*)a);
	
	app_regs.REG_PULSE_ENABLE = reg;
	return true;
}


/************************************************************************/
/* REG_PULSE_DCYCLE_LED0                                              */
/************************************************************************/
void app_read_REG_PULSE_DCYCLE_LED0(void) {}
bool app_write_REG_PULSE_DCYCLE_LED0(void *a)
{
	uint8_t reg = *((uint8_t*)a);

	if (reg < 1 || reg > 100)
	{
		return false;
	}

	app_regs.REG_PULSE_DCYCLE_LED0 = reg;
	return true;
}


/************************************************************************/
/* REG_PULSE_DCYCLE_LED1                                              */
/************************************************************************/
void app_read_REG_PULSE_DCYCLE_LED1(void) {}
bool app_write_REG_PULSE_DCYCLE_LED1(void *a)
{
	uint8_t reg = *((uint8_t*)a);

	if (reg < 1 || reg > 100)
	{
		return false;
	}

	app_regs.REG_PULSE_DCYCLE_LED1 = reg;
	return true;
}


/************************************************************************/
/* REG_PULSE_FREQUENCY_LED0                                             */
/************************************************************************/
void app_read_REG_PULSE_FREQUENCY_LED0(void){}
bool app_write_REG_PULSE_FREQUENCY_LED0(void *a)
{
	uint8_t reg = *((uint8_t*)a);

	if (reg == 0)
	{
		return false;
	}

	app_regs.REG_PULSE_FREQUENCY_LED0 = reg;
	return true;
}


/************************************************************************/
/* REG_PULSE_FREQUENCY_LED1                                             */
/************************************************************************/
void app_read_REG_PULSE_FREQUENCY_LED1(void){}
bool app_write_REG_PULSE_FREQUENCY_LED1(void *a)
{
	uint8_t reg = *((uint8_t*)a);

	if (reg == 0)
	{
		return false;
	}

	app_regs.REG_PULSE_FREQUENCY_LED1 = reg;
	return true;
}


/************************************************************************/
/* REG_RAMP_LED0                                                        */
/************************************************************************/
void app_read_REG_RAMP_LED0(void) {}
bool app_write_REG_RAMP_LED0(void *a)
{
	uint16_t reg = *((uint16_t*)a);

	if (reg == 0)
	{
		return false;
	}

	app_regs.REG_RAMP_LED0 = reg;
	return true;
}


/************************************************************************/
/* REG_RAMP_LED1                                                        */
/************************************************************************/
void app_read_REG_RAMP_LED1(void) {}
bool app_write_REG_RAMP_LED1(void *a)
{
	uint16_t reg = *((uint16_t*)a);

	if (reg == 0)
	{
		return false;
	}

	app_regs.REG_RAMP_LED1 = reg;
	return true;
}


/************************************************************************/
/* REG_RAMP_CONFIG                                                      */
/************************************************************************/
void app_read_REG_RAMP_CONFIG(void) {}
bool app_write_REG_RAMP_CONFIG(void *a)
{
	uint8_t reg = *((uint8_t*)a);

	app_regs.REG_RAMP_CONFIG = reg;
	return true;
}


/************************************************************************/
/* REG_PROTOCOL0_DURATION                                               */
/************************************************************************/
void app_read_REG_PROTOCOL0_DURATION(void) {}
bool app_write_REG_PROTOCOL0_DURATION(void *a)
{
	uint16_t reg = *((uint16_t*)a);

	app_regs.REG_PROTOCOL0_DURATION = reg;
	return true;
}


/************************************************************************/
/* REG_PROTOCOL1_DURATION                                               */
/************************************************************************/
void app_read_REG_PROTOCOL1_DURATION(void) {}
bool app_write_REG_PROTOCOL1_DURATION(void *a)
{
	uint16_t reg = *((uint16_t*)a);

	app_regs.REG_PROTOCOL1_DURATION = reg;
	return true;
}


/************************************************************************/
/* REG_PROTOCOL0_DELAY                                                  */
/************************************************************************/
void app_read_REG_PROTOCOL0_DELAY(void) {}
bool app_write_REG_PROTOCOL0_DELAY(void *a)
{
	uint16_t reg = *((uint16_t*)a);

	app_regs.REG_PROTOCOL0_DELAY = reg;
	return true;
}


/************************************************************************/
/* REG_PROTOCOL1_DELAY                                                  */
/************************************************************************/
void app_read_REG_PROTOCOL1_DELAY(void) {}
bool app_write_REG_PROTOCOL1_DELAY(void *a)
{
	uint16_t reg = *((uint16_t*)a);

	app_regs.REG_PROTOCOL1_DELAY = reg;
	return true;
}


/************************************************************************/
/* REG_ENABLE_PROTOCOL                                                  */
/************************************************************************/
void app_read_REG_ENABLE_PROTOCOL(void) {}
bool app_write_REG_ENABLE_PROTOCOL(void *a)
{
	uint8_t reg = *((uint8_t*)a);

	if (reg & B_LED0)
	{
		protocol0.pulses.use_pulses = (bool)(app_regs.REG_PULSE_ENABLE & B_LED0);
		protocol0.pulses.state = false;
		protocol0.pulses.time_on = app_regs.REG_PULSE_DCYCLE_LED0 / (100.0 * app_regs.REG_PULSE_FREQUENCY_LED0) * 1000.0 + 1;
		protocol0.pulses.time_off = 1000.0 / app_regs.REG_PULSE_FREQUENCY_LED0 - protocol0.pulses.time_on + 1;
		protocol0.pulses.countdown = protocol0.pulses.time_on;
		
		protocol0.ramps.use_ramps = protocol0.pulses.use_pulses ? 0 : app_regs.REG_RAMP_CONFIG;
		protocol0.ramps.cycle_amount = (uint16_t)((app_regs.REG_DAC0_VOLTAGE / 5000  * 65535) / app_regs.REG_RAMP_LED0);
		protocol0.ramps.remainder_rise = (uint16_t)((app_regs.REG_DAC0_VOLTAGE / 5000 * 65535) - protocol0.ramps.cycle_amount * app_regs.REG_RAMP_LED0);
		protocol0.ramps.remainder_fall = (uint16_t)((app_regs.REG_DAC0_VOLTAGE / 5000 * 65535) - protocol0.ramps.cycle_amount * app_regs.REG_RAMP_LED0);
		protocol0.ramps.previous_value = 0;
		protocol0.ramps.countdown_rise = app_regs.REG_RAMP_LED0;
		protocol0.ramps.countdown_fall = app_regs.REG_RAMP_LED0;

		protocol0.delay = app_regs.REG_PROTOCOL0_DELAY;
		protocol0.duration = app_regs.REG_PROTOCOL0_DURATION;
		protocol0.has_duration = (bool)(app_regs.REG_PROTOCOL0_DURATION);
		protocol0.target = app_regs.REG_DAC0_VOLTAGE / 5000 * 65535;
		
		if (protocol0.delay != 0)
		{
			protocol0.state = DELAY;
		} else if (protocol0.ramps.use_ramps & B_LED0_RISE) {
			protocol0.state = RISE;
		} else {
			protocol0.state = ON;
		}
	}

	if (reg & B_LED1)
	{
		protocol1.pulses.use_pulses = (bool)(app_regs.REG_PULSE_ENABLE & B_LED1);
		protocol1.pulses.state = false;
		protocol1.pulses.time_on = app_regs.REG_PULSE_DCYCLE_LED1 / (100.0 * app_regs.REG_PULSE_FREQUENCY_LED1) * 1000.0 + 1;
		protocol1.pulses.time_off = 1000.0 / app_regs.REG_PULSE_FREQUENCY_LED1 - protocol1.pulses.time_on + 1;
		protocol1.pulses.countdown = protocol1.pulses.time_on;
		
		protocol1.ramps.use_ramps = protocol1.pulses.use_pulses ? 0 : app_regs.REG_RAMP_CONFIG;
		protocol1.ramps.cycle_amount = (uint16_t)((app_regs.REG_DAC1_VOLTAGE / 5000  * 65535) / app_regs.REG_RAMP_LED1);
		protocol1.ramps.remainder_rise = (uint16_t)((app_regs.REG_DAC1_VOLTAGE / 5000 * 65535) - protocol1.ramps.cycle_amount * app_regs.REG_RAMP_LED1);
		protocol1.ramps.remainder_fall = (uint16_t)((app_regs.REG_DAC1_VOLTAGE / 5000 * 65535) - protocol1.ramps.cycle_amount * app_regs.REG_RAMP_LED1);
		protocol1.ramps.previous_value = 0;
		protocol1.ramps.countdown_rise = app_regs.REG_RAMP_LED1;
		protocol1.ramps.countdown_fall = app_regs.REG_RAMP_LED1;

		protocol1.delay = app_regs.REG_PROTOCOL1_DELAY;
		protocol1.duration = app_regs.REG_PROTOCOL1_DURATION;
		protocol1.has_duration = (bool)(app_regs.REG_PROTOCOL1_DURATION);
		protocol1.target = app_regs.REG_DAC1_VOLTAGE / 5000 * 65535;
		
		if (protocol1.delay != 0)
		{
			protocol1.state = DELAY;
		} else if (protocol1.ramps.use_ramps & B_LED1_RISE) {
			protocol1.state = RISE;
		} else {
			protocol1.state = ON;
		}
	}

	app_regs.REG_ENABLE_PROTOCOL = reg;
	return true;
}


/************************************************************************/
/* REG_DISABLE_PROTOCOL                                                 */
/************************************************************************/
void app_read_REG_DISABLE_PROTOCOL(void) {}
bool app_write_REG_DISABLE_PROTOCOL(void *a)
{
	uint8_t reg = *((uint8_t*)a);

	if (reg & B_LED0) {
		if (protocol0.ramps.use_ramps & B_LED0_FALL && (protocol0.state == RISE || protocol0.state == FALL)) {
			protocol0.state = FALL;
		} else if (protocol0.ramps.use_ramps & B_LED0_FALL && protocol0.state == ON) {
			protocol0.ramps.previous_value = protocol0.target;
			protocol0.state = FALL;
		} else {
			latch_dac0(0);
			protocol0.state = OFF;
		}
	}
	
	if (reg & B_LED1) {
		latch_dac1(0);
		protocol1.state = OFF;
	}

	app_regs.REG_DISABLE_PROTOCOL = reg;
	return true;
}


/************************************************************************/
/* REG_DI0_TRIGGER                                                      */
/************************************************************************/
void app_read_REG_DI0_TRIGGER(void) {}
bool app_write_REG_DI0_TRIGGER(void *a)
{
	uint8_t reg = *((uint8_t*)a);

	app_regs.REG_DI0_TRIGGER = reg;
	return true;
}


/************************************************************************/
/* REG_DI1_TRIGGER                                                      */
/************************************************************************/
void app_read_REG_DI1_TRIGGER(void) {}
bool app_write_REG_DI1_TRIGGER(void *a)
{
	uint8_t reg = *((uint8_t*)a);

	app_regs.REG_DI1_TRIGGER = reg;
	return true;
}


/************************************************************************/
/* REG_RESERVED0                                                        */
/************************************************************************/
void app_read_REG_RESERVED0(void) {}
bool app_write_REG_RESERVED0(void *a)
{
	uint8_t reg = *((uint8_t*)a);

	app_regs.REG_RESERVED0 = reg;
	return true;
}


/************************************************************************/
/* REG_RESERVED1                                                        */
/************************************************************************/
void app_read_REG_RESERVED1(void) {}
bool app_write_REG_RESERVED1(void *a)
{
	uint8_t reg = *((uint8_t*)a);

	app_regs.REG_RESERVED1 = reg;
	return true;
}


/************************************************************************/
/* REG_RESERVED2                                                        */
/************************************************************************/
void app_read_REG_RESERVED2(void) {}
bool app_write_REG_RESERVED2(void *a)
{
	uint8_t reg = *((uint8_t*)a);

	app_regs.REG_RESERVED2 = reg;
	return true;
}


/************************************************************************/
/* REG_RESERVED3                                                        */
/************************************************************************/
void app_read_REG_RESERVED3(void) {}
bool app_write_REG_RESERVED3(void *a)
{
	uint8_t reg = *((uint8_t*)a);

	app_regs.REG_RESERVED3 = reg;
	return true;
}


/************************************************************************/
/* REG_EVNT_ENABLE                                                      */
/************************************************************************/
void app_read_REG_EVNT_ENABLE(void) {}
bool app_write_REG_EVNT_ENABLE(void *a)
{
	uint8_t reg = *((uint8_t*)a);

	app_regs.REG_EVNT_ENABLE = reg;
	
	return true;
}
