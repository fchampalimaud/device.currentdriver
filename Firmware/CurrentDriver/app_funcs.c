#include "app_funcs.h"
#include "app_ios_and_regs.h"
#include "hwbp_core.h"


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
	&app_read_REG_LED0_CURRENT,
	&app_read_REG_LED1_CURRENT,
	&app_read_REG_DAC0_VOLTAGE,
	&app_read_REG_DAC1_VOLTAGE,
	&app_read_REG_LED_ENABLE,
	&app_read_REG_LED_DISABLE,
	&app_read_REG_LED_OUT,
	&app_read_REG_LED0_MAX_CURRENT,
	&app_read_REG_LED1_MAX_CURRENT,
	&app_read_REG_RESERVED0,
	&app_read_REG_RESERVED1,
	&app_read_REG_RESERVED2,
	&app_read_REG_RESERVED3,
	&app_read_REG_RESERVED4,
	&app_read_REG_EVNT_ENABLE
};

bool (*app_func_wr_pointer[])(void*) = {
	&app_write_REG_PORT_DIS,
	&app_write_REG_OUTPUTS_SET,
	&app_write_REG_OUTPUTS_CLEAR,
	&app_write_REG_OUTPUTS_TOGGLE,
	&app_write_REG_OUTPUTS_OUT,
	&app_write_REG_LED0_CURRENT,
	&app_write_REG_LED1_CURRENT,
	&app_write_REG_DAC0_VOLTAGE,
	&app_write_REG_DAC1_VOLTAGE,
	&app_write_REG_LED_ENABLE,
	&app_write_REG_LED_DISABLE,
	&app_write_REG_LED_OUT,
	&app_write_REG_LED0_MAX_CURRENT,
	&app_write_REG_LED1_MAX_CURRENT,
	&app_write_REG_RESERVED0,
	&app_write_REG_RESERVED1,
	&app_write_REG_RESERVED2,
	&app_write_REG_RESERVED3,
	&app_write_REG_RESERVED4,
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
/* REG_LED0_CURRENT                                                     */
/************************************************************************/
uint16_t dummy_u16b;
void load_dac(uint16_t word, int pin)
{
	// Turn off CE?
	clear_io(PORTD, pin);
	
	dummy_u16b = word;
	
	// Sends one byte
	SPID_DATA = *(((uint8_t*)(&dummy_u16b))+1);
	loop_until_bit_is_set(SPID_STATUS, SPI_IF_bp);

	// Sends the other byte
	SPID_DATA = *(((uint8_t*)(&dummy_u16b))+0);
	loop_until_bit_is_set(SPID_STATUS, SPI_IF_bp);

	// Shouldn't I turn on the CE again?
}

void app_read_REG_LED0_CURRENT(void) {}
bool app_write_REG_LED0_CURRENT(void *a)
{
	float reg = *((float*)a);
	
	if (reg < 0 || (reg > 1000 && reg > app_regs.REG_LED0_MAX_CURRENT))
	{
		return false;
	}

	uint16_t daqValue = (uint16_t)(reg / 1000  * 65535);
	load_dac(daqValue, 3);

	app_regs.REG_LED0_CURRENT = reg;
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
	
	uint16_t daqValue = (uint16_t)(reg / 1000  * 65535);
	load_dac(daqValue, 4);

	app_regs.REG_LED1_CURRENT = reg;
	return true;
}


/************************************************************************/
/* REG_DAC0_VOLTAGE                                                     */
/************************************************************************/
void app_read_REG_DAC0_VOLTAGE(void) {}
bool app_write_REG_DAC0_VOLTAGE(void *a)
{
	float reg = *((float*)a);

	if (reg < 0 || reg > 4000)
	{
		return false;
	}

	uint16_t daqValue = (uint16_t)(reg / 4000  * 65535);
	load_dac(daqValue, 3);

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

	if (reg < 0 || reg > 4000)
	{
		return false;
	}

	uint16_t daqValue = (uint16_t)(reg / 4000  * 65535);
	load_dac(daqValue, 4);

	app_regs.REG_DAC1_VOLTAGE = reg;
	return true;
}


/************************************************************************/
/* REG_LED_ENABLE                                                       */
/************************************************************************/
void app_read_REG_LED_ENABLE(void) {}
bool app_write_REG_LED_ENABLE(void *a)
{
	uint8_t reg = *((uint8_t*)a);

	if (reg & B_LED0) clr_I_OFF_DAC0;
	if (reg & B_LED1) clr_I_OFF_DAC1;

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

	if (reg & B_LED0) clr_I_OFF_DAC0; else set_I_OFF_DAC0;
	if (reg & B_LED1) clr_I_OFF_DAC1; else set_I_OFF_DAC1;

	app_regs.REG_LED_OUT = reg;
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

	app_regs.REG_LED1_MAX_CURRENT = reg;
	
	return true;
}


/************************************************************************/
/* REG_RESERVED0                                                        */
/************************************************************************/
void app_read_REG_RESERVED0(void)
{
	//app_regs.REG_RESERVED0 = 0;

}

bool app_write_REG_RESERVED0(void *a)
{
	uint8_t reg = *((uint8_t*)a);

	app_regs.REG_RESERVED0 = reg;
	return true;
}


/************************************************************************/
/* REG_RESERVED1                                                        */
/************************************************************************/
void app_read_REG_RESERVED1(void)
{
	//app_regs.REG_RESERVED1 = 0;

}

bool app_write_REG_RESERVED1(void *a)
{
	uint8_t reg = *((uint8_t*)a);

	app_regs.REG_RESERVED1 = reg;
	return true;
}


/************************************************************************/
/* REG_RESERVED2                                                        */
/************************************************************************/
void app_read_REG_RESERVED2(void)
{
	//app_regs.REG_RESERVED2 = 0;

}

bool app_write_REG_RESERVED2(void *a)
{
	uint8_t reg = *((uint8_t*)a);

	app_regs.REG_RESERVED2 = reg;
	return true;
}


/************************************************************************/
/* REG_RESERVED3                                                        */
/************************************************************************/
void app_read_REG_RESERVED3(void)
{
	//app_regs.REG_RESERVED3 = 0;

}

bool app_write_REG_RESERVED3(void *a)
{
	uint8_t reg = *((uint8_t*)a);

	app_regs.REG_RESERVED3 = reg;
	return true;
}


/************************************************************************/
/* REG_RESERVED4                                                        */
/************************************************************************/
void app_read_REG_RESERVED4(void)
{
	//app_regs.REG_RESERVED4 = 0;

}

bool app_write_REG_RESERVED4(void *a)
{
	uint8_t reg = *((uint8_t*)a);

	app_regs.REG_RESERVED4 = reg;
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