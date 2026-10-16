#include "cpu.h"
#include "hwbp_core_types.h"
#include "app_ios_and_regs.h"
#include "app_funcs.h"
#include "hwbp_core.h"
#include "app.h"
#include "structs.h"


/************************************************************************/
/* Declare application registers                                        */
/************************************************************************/
extern AppRegs app_regs;

extern Protocol protocol0;
extern Protocol protocol1;

/************************************************************************/
/* Interrupts from Timers                                               */
/************************************************************************/
// ISR(TCC0_OVF_vect, ISR_NAKED)
// ISR(TCD0_OVF_vect, ISR_NAKED)
// ISR(TCE0_OVF_vect, ISR_NAKED)
// ISR(TCF0_OVF_vect, ISR_NAKED)
// 
// ISR(TCC0_CCA_vect, ISR_NAKED)
// ISR(TCD0_CCA_vect, ISR_NAKED)
// ISR(TCE0_CCA_vect, ISR_NAKED)
// ISR(TCF0_CCA_vect, ISR_NAKED)
// 
// ISR(TCD1_OVF_vect, ISR_NAKED)
// 
// ISR(TCD1_CCA_vect, ISR_NAKED)

/************************************************************************/ 
/* DI0                                                                  */
/************************************************************************/
ISR(PORTB_INT0_vect, ISR_NAKED)
{
	uint8_t reg_port_dis = app_regs.REG_PORT_DIS;
	
	app_regs.REG_PORT_DIS &= ~B_DI0;
	app_regs.REG_PORT_DIS |= (read_DI0) ? B_DI0 : 0;

	if (app_regs.REG_PORT_DIS & B_DI0 && (app_regs.REG_CHANNEL0_MODE == GM_START_PROTOCOL_TRIGGER || app_regs.REG_CHANNEL0_MODE == GM_START_AND_STOP_PROTOCOL_TRIGGER))
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
		protocol0.target = app_regs.REG_DAC0_VOLTAGE / 5000 * 65535;

		protocol0.has_duration = false;
		if (app_regs.REG_CHANNEL0_MODE == GM_START_PROTOCOL_TRIGGER) {
			protocol0.has_duration = (bool)(app_regs.REG_PROTOCOL0_DURATION);
		}
		
		if (protocol0.delay != 0)
		{
			protocol0.state = DELAY;
		} else if (protocol0.ramps.use_ramps & B_LED0_RISE) {
			protocol0.state = RISE;
		} else {
			protocol0.state = ON;
		}
	}

	if (!(app_regs.REG_PORT_DIS & B_DI0) && app_regs.REG_CHANNEL0_MODE == GM_START_AND_STOP_PROTOCOL_TRIGGER) {
		if (protocol0.ramps.use_ramps & B_LED0_FALL) {
			protocol0.ramps.previous_value = protocol0.target;
			protocol0.state = FALL;
		} else {
			latch_dac0(0);
			protocol0.state = OFF;
		}
	}
	
	if (app_regs.REG_EVNT_ENABLE & B_EVT_PORT_DIS)
	{
		if (reg_port_dis != app_regs.REG_PORT_DIS)
		{
			core_func_send_event(ADD_REG_PORT_DIS, true);
		}
	}

	reti();
}

/************************************************************************/ 
/* HOLD0 & HOLD1 & CE1 & CE2                                            */
/************************************************************************/
ISR(PORTD_INT0_vect, ISR_NAKED)
{
	reti();
}

/************************************************************************/ 
/* DI1                                                                  */
/************************************************************************/
ISR(PORTH_INT0_vect, ISR_NAKED)
{
	uint8_t reg_port_dis = app_regs.REG_PORT_DIS;
	
	app_regs.REG_PORT_DIS &= ~B_DI1;
	app_regs.REG_PORT_DIS |= (read_DI1) ? B_DI1 : 0;

	if (app_regs.REG_PORT_DIS & B_DI1 && (app_regs.REG_CHANNEL1_MODE == GM_START_PROTOCOL_TRIGGER || app_regs.REG_CHANNEL1_MODE == GM_START_AND_STOP_PROTOCOL_TRIGGER))
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
		protocol1.target = app_regs.REG_DAC1_VOLTAGE / 5000 * 65535;

		protocol1.has_duration = false;
		if (app_regs.REG_CHANNEL1_MODE == GM_START_PROTOCOL_TRIGGER) {
			protocol0.has_duration = (bool)(app_regs.REG_PROTOCOL1_DURATION);
		}
		
		if (protocol1.delay != 0)
		{
			protocol1.state = DELAY;
		} else if (protocol1.ramps.use_ramps & B_LED1_RISE) {
			protocol1.state = RISE;
		} else {
			protocol1.state = ON;
		}
	}
	
	if (!(app_regs.REG_PORT_DIS & B_DI1) && app_regs.REG_CHANNEL1_MODE == GM_START_AND_STOP_PROTOCOL_TRIGGER) {
		if (protocol1.ramps.use_ramps & B_LED1_FALL) {
				protocol1.ramps.previous_value = protocol1.target;
				protocol1.state = FALL;
		} else {
			latch_dac1(0);
			protocol1.state = OFF;
		}
	}

	if (app_regs.REG_EVNT_ENABLE & B_EVT_PORT_DIS)
	{
		if (reg_port_dis != app_regs.REG_PORT_DIS)
		{
			core_func_send_event(ADD_REG_PORT_DIS, true);
		}
	}
	
	reti();
}

