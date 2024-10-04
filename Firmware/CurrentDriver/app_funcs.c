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
	//app_regs.REG_PORT_DIS = 0;

}

bool app_write_REG_PORT_DIS(void *a)
{
	uint8_t reg = *((uint8_t*)a);

	app_regs.REG_PORT_DIS = reg;
	return true;
}


/************************************************************************/
/* REG_OUTPUTS_SET                                                      */
/************************************************************************/
void app_read_REG_OUTPUTS_SET(void)
{
	//app_regs.REG_OUTPUTS_SET = 0;

}

bool app_write_REG_OUTPUTS_SET(void *a)
{
	uint16_t reg = *((uint16_t*)a);

	app_regs.REG_OUTPUTS_SET = reg;
	return true;
}


/************************************************************************/
/* REG_OUTPUTS_CLEAR                                                    */
/************************************************************************/
void app_read_REG_OUTPUTS_CLEAR(void)
{
	//app_regs.REG_OUTPUTS_CLEAR = 0;

}

bool app_write_REG_OUTPUTS_CLEAR(void *a)
{
	uint16_t reg = *((uint16_t*)a);

	app_regs.REG_OUTPUTS_CLEAR = reg;
	return true;
}


/************************************************************************/
/* REG_OUTPUTS_TOGGLE                                                   */
/************************************************************************/
void app_read_REG_OUTPUTS_TOGGLE(void)
{
	//app_regs.REG_OUTPUTS_TOGGLE = 0;

}

bool app_write_REG_OUTPUTS_TOGGLE(void *a)
{
	uint16_t reg = *((uint16_t*)a);

	app_regs.REG_OUTPUTS_TOGGLE = reg;
	return true;
}


/************************************************************************/
/* REG_OUTPUTS_OUT                                                      */
/************************************************************************/
void app_read_REG_OUTPUTS_OUT(void)
{
	//app_regs.REG_OUTPUTS_OUT = 0;

}

bool app_write_REG_OUTPUTS_OUT(void *a)
{
	uint16_t reg = *((uint16_t*)a);

	app_regs.REG_OUTPUTS_OUT = reg;
	return true;
}


/************************************************************************/
/* REG_LED0_CURRENT                                                     */
/************************************************************************/
void app_read_REG_LED0_CURRENT(void)
{
	//app_regs.REG_LED0_CURRENT = 0;

}

bool app_write_REG_LED0_CURRENT(void *a)
{
	float reg = *((float*)a);

	app_regs.REG_LED0_CURRENT = reg;
	return true;
}


/************************************************************************/
/* REG_LED1_CURRENT                                                     */
/************************************************************************/
void app_read_REG_LED1_CURRENT(void)
{
	//app_regs.REG_LED1_CURRENT = 0;

}

bool app_write_REG_LED1_CURRENT(void *a)
{
	float reg = *((float*)a);

	app_regs.REG_LED1_CURRENT = reg;
	return true;
}


/************************************************************************/
/* REG_DAC0_VOLTAGE                                                     */
/************************************************************************/
void app_read_REG_DAC0_VOLTAGE(void)
{
	//app_regs.REG_DAC0_VOLTAGE = 0;

}

bool app_write_REG_DAC0_VOLTAGE(void *a)
{
	float reg = *((float*)a);

	app_regs.REG_DAC0_VOLTAGE = reg;
	return true;
}


/************************************************************************/
/* REG_DAC1_VOLTAGE                                                     */
/************************************************************************/
void app_read_REG_DAC1_VOLTAGE(void)
{
	//app_regs.REG_DAC1_VOLTAGE = 0;

}

bool app_write_REG_DAC1_VOLTAGE(void *a)
{
	float reg = *((float*)a);

	app_regs.REG_DAC1_VOLTAGE = reg;
	return true;
}


/************************************************************************/
/* REG_LED_ENABLE                                                       */
/************************************************************************/
void app_read_REG_LED_ENABLE(void)
{
	//app_regs.REG_LED_ENABLE = 0;

}

bool app_write_REG_LED_ENABLE(void *a)
{
	uint8_t reg = *((uint8_t*)a);

	app_regs.REG_LED_ENABLE = reg;
	return true;
}


/************************************************************************/
/* REG_LED_DISABLE                                                      */
/************************************************************************/
void app_read_REG_LED_DISABLE(void)
{
	//app_regs.REG_LED_DISABLE = 0;

}

bool app_write_REG_LED_DISABLE(void *a)
{
	uint8_t reg = *((uint8_t*)a);

	app_regs.REG_LED_DISABLE = reg;
	return true;
}


/************************************************************************/
/* REG_LED_OUT                                                          */
/************************************************************************/
void app_read_REG_LED_OUT(void)
{
	//app_regs.REG_LED_OUT = 0;

}

bool app_write_REG_LED_OUT(void *a)
{
	uint8_t reg = *((uint8_t*)a);

	app_regs.REG_LED_OUT = reg;
	return true;
}


/************************************************************************/
/* REG_LED0_MAX_CURRENT                                                 */
/************************************************************************/
void app_read_REG_LED0_MAX_CURRENT(void)
{
	//app_regs.REG_LED0_MAX_CURRENT = 0;

}

bool app_write_REG_LED0_MAX_CURRENT(void *a)
{
	uint8_t reg = *((uint8_t*)a);

	app_regs.REG_LED0_MAX_CURRENT = reg;
	return true;
}


/************************************************************************/
/* REG_LED1_MAX_CURRENT                                                 */
/************************************************************************/
void app_read_REG_LED1_MAX_CURRENT(void)
{
	//app_regs.REG_LED1_MAX_CURRENT = 0;

}

bool app_write_REG_LED1_MAX_CURRENT(void *a)
{
	uint8_t reg = *((uint8_t*)a);

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
void app_read_REG_EVNT_ENABLE(void)
{
	//app_regs.REG_EVNT_ENABLE = 0;

}

bool app_write_REG_EVNT_ENABLE(void *a)
{
	uint8_t reg = *((uint8_t*)a);

	app_regs.REG_EVNT_ENABLE = reg;
	return true;
}