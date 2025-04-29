#include "cpu.h"
#include "hwbp_core_types.h"
#include "app_ios_and_regs.h"
#include "app_funcs.h"
#include "hwbp_core.h"

/************************************************************************/
/* Declare application registers                                        */
/************************************************************************/
extern AppRegs app_regs;

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
	
	if (app_regs.REG_EVNT_ENABLE & B_EVT_PORT_DIS)
	{
		if (reg_port_dis != app_regs.REG_PORT_DIS)
		{
			core_func_send_event(ADD_REG_PORT_DIS, true);
		}
	}
	
	reti();
}

