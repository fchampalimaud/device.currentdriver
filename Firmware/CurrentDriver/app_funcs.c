#include "app_funcs.h"
#include "app_ios_and_regs.h"
#include "hwbp_core.h"

#define F_CPU 32000000
#include <util/delay.h>

#include "structs.h"


/************************************************************************/
/* Create pointers to functions                                         */
/************************************************************************/
extern AppRegs app_regs;

/* Current and voltage writes share validation, conversion and execution. */
static bool channel_write_setpoint(uint8_t channel, float value, bool current)
{
    float max_current = channel ? app_regs.REG_LED1_MAX_CURRENT : app_regs.REG_LED0_MAX_CURRENT;
    bool driver_enabled = channel ? !read_I_OFF_DAC1 : !read_I_OFF_DAC0;
    float voltage;
    uint8_t mode = channel ? app_regs.REG_CHANNEL1_MODE : app_regs.REG_CHANNEL0_MODE;

    /* Ordered comparisons also reject NaN and infinities. */
    if (!(value >= 0 && value <= (current ? 500.0f : 5000.0f)))
        return false;
    voltage = current ? value * 10.0f : value;
    if ((current || driver_enabled) && voltage / 10.0f > max_current)
        return false;
    if (channel) {
        app_regs.REG_LED1_CURRENT = voltage / 10.0f;
        app_regs.REG_DAC1_VOLTAGE = voltage;
    } else {
        app_regs.REG_LED0_CURRENT = voltage / 10.0f;
        app_regs.REG_DAC0_VOLTAGE = voltage;
    }
    if (mode == GM_MANUAL)
        channel_apply_manual(channel);
    return true;
}


void (*app_func_rd_pointer[])(void) = {
	&app_read_REG_PORT_DIS,
	&app_read_REG_OUTPUTS_SET,
	&app_read_REG_OUTPUTS_CLEAR,
	&app_read_REG_OUTPUTS_TOGGLE,
	&app_read_REG_OUTPUTS_OUT,
	&app_read_REG_LED_ENABLE,
	&app_read_REG_LED_DISABLE,
	&app_read_REG_LED_OUT,
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
	&app_read_REG_CHANNEL0_MODE,
	&app_read_REG_CHANNEL1_MODE,
	&app_read_REG_CHANNEL_STATE,
	&app_read_REG_RESERVED0,
	&app_read_REG_RESERVED1,
	&app_read_REG_RESERVED2,
	&app_read_REG_RESERVED3,
	&app_read_REG_RESERVED4,
	&app_read_REG_EVNT_ENABLE};

bool (*app_func_wr_pointer[])(void *) = {
	&app_write_REG_PORT_DIS,
	&app_write_REG_OUTPUTS_SET,
	&app_write_REG_OUTPUTS_CLEAR,
	&app_write_REG_OUTPUTS_TOGGLE,
	&app_write_REG_OUTPUTS_OUT,
	&app_write_REG_LED_ENABLE,
	&app_write_REG_LED_DISABLE,
	&app_write_REG_LED_OUT,
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
	&app_write_REG_CHANNEL0_MODE,
	&app_write_REG_CHANNEL1_MODE,
	&app_write_REG_CHANNEL_STATE,
	&app_write_REG_RESERVED0,
	&app_write_REG_RESERVED1,
	&app_write_REG_RESERVED2,
	&app_write_REG_RESERVED3,
	&app_write_REG_RESERVED4,
	&app_write_REG_EVNT_ENABLE};

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
	uint16_t reg = *((uint16_t *)a);

	if (reg & B_DO0)
		set_DO0;
	if (reg & B_DO1)
		set_DO1;

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
	uint16_t reg = *((uint16_t *)a);

	if (reg & B_DO0)
		clr_DO0;
	if (reg & B_DO1)
		clr_DO1;

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
	uint16_t reg = *((uint16_t *)a);

	if (reg & B_DO0)
	{
		tgl_DO0;
	}
	if (reg & B_DO1)
	{
		tgl_DO1;
	}

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
	uint16_t reg = *((uint16_t *)a);

	if (reg & B_DO0)
		set_DO0;
	else
		clr_DO0;
	if (reg & B_DO1)
		set_DO1;
	else
		clr_DO1;

	app_regs.REG_OUTPUTS_OUT = reg;

	return true;
}

/************************************************************************/
/* LATCH_DAC0                                                           */
/************************************************************************/
void latch_dac0(uint16_t word)
{
	clr_CS0;

	// 16 bit code latch
	SPID_DATA = *(((uint8_t *)(&word)) + 1);
	loop_until_bit_is_set(SPID_STATUS, SPI_IF_bp);

	SPID_DATA = *(((uint8_t *)(&word)) + 0);
	loop_until_bit_is_set(SPID_STATUS, SPI_IF_bp);

	set_CS0;
}

/************************************************************************/
/* LATCH_DAC1                                                           */
/************************************************************************/
void latch_dac1(uint16_t word)
{
	clr_CS1;

	// 16 bit code latch
	SPID_DATA = *(((uint8_t *)(&word)) + 1);
	loop_until_bit_is_set(SPID_STATUS, SPI_IF_bp);

	SPID_DATA = *(((uint8_t *)(&word)) + 0);
	loop_until_bit_is_set(SPID_STATUS, SPI_IF_bp);

	set_CS1;
}

/************************************************************************/
/* REG_LED_ENABLE                                                       */
/************************************************************************/
void app_read_REG_LED_ENABLE(void) {}
bool app_write_REG_LED_ENABLE(void *a)
{
	uint8_t reg = *((uint8_t *)a);

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
	uint8_t reg = *((uint8_t *)a);

	if (reg & B_LED0)
		set_I_OFF_DAC0;
	if (reg & B_LED1)
		set_I_OFF_DAC1;

	app_regs.REG_LED_OUT |= reg;
	app_regs.REG_LED_DISABLE = reg;

	return true;
}

/************************************************************************/
/* REG_LED_OUT                                                          */
/************************************************************************/
void app_read_REG_LED_OUT(void)
{
	app_regs.REG_OUTPUTS_OUT = (read_I_OFF_DAC0) ? B_LED0 : 0;
	app_regs.REG_OUTPUTS_OUT |= (read_I_OFF_DAC1) ? B_LED1 : 0;
}

bool app_write_REG_LED_OUT(void *a)
{
	uint8_t reg = *((uint8_t *)a);

	if (reg & B_LED0)
	{
		if (app_regs.REG_LED0_CURRENT > app_regs.REG_LED0_MAX_CURRENT)
		{
			app_regs.REG_LED0_CURRENT = app_regs.REG_LED0_MAX_CURRENT;
			app_regs.REG_DAC0_VOLTAGE = 5 * app_regs.REG_LED0_MAX_CURRENT;
		}
		clr_I_OFF_DAC0;
	}
	else
	{
		set_I_OFF_DAC0;
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
	else
	{
		set_I_OFF_DAC1;
	}

	app_regs.REG_LED_OUT = reg;

	return true;
}

/************************************************************************/
/* REG_LED0_CURRENT                                                     */
/************************************************************************/
void app_read_REG_LED0_CURRENT(void) {}
bool app_write_REG_LED0_CURRENT(void *a)
{
    return channel_write_setpoint(0, *((float *)a), true);
}

/************************************************************************/
/* REG_LED1_CURRENT                                                     */
/************************************************************************/
void app_read_REG_LED1_CURRENT(void) {}
bool app_write_REG_LED1_CURRENT(void *a)
{
    return channel_write_setpoint(1, *((float *)a), true);
}

/************************************************************************/
/* REG_LED0_MAX_CURRENT                                                 */
/************************************************************************/
void app_read_REG_LED0_MAX_CURRENT(void) {}
bool app_write_REG_LED0_MAX_CURRENT(void *a)
{
	float reg = *((float *)a);

	if (reg < 1 || reg > 500)
	{
		return false;
	}

	if (app_regs.REG_LED0_CURRENT > reg && app_regs.REG_LED_OUT & B_LED0)
	{
		app_regs.REG_LED0_CURRENT = reg;
		app_regs.REG_DAC0_VOLTAGE = 10 * reg;
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
	float reg = *((float *)a);

	if (reg < 1 || reg > 500)
	{
		return false;
	}

	if (app_regs.REG_LED1_CURRENT > reg && app_regs.REG_LED_OUT & B_LED1)
	{
		app_regs.REG_LED1_CURRENT = reg;
		app_regs.REG_DAC1_VOLTAGE = 10 * reg;
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
    return channel_write_setpoint(0, *((float *)a), false);
}

/************************************************************************/
/* REG_DAC1_VOLTAGE                                                     */
/************************************************************************/
void app_read_REG_DAC1_VOLTAGE(void) {}
bool app_write_REG_DAC1_VOLTAGE(void *a)
{
    return channel_write_setpoint(1, *((float *)a), false);
}

/************************************************************************/
/* REG_PULSE_ENABLE                                                     */
/************************************************************************/
void app_read_REG_PULSE_ENABLE(void) {}
bool app_write_REG_PULSE_ENABLE(void *a)
{
	uint8_t reg = *((uint8_t *)a);

	app_regs.REG_PULSE_ENABLE = reg;
	return true;
}

/************************************************************************/
/* REG_PULSE_DCYCLE_LED0                                              */
/************************************************************************/
void app_read_REG_PULSE_DCYCLE_LED0(void) {}
bool app_write_REG_PULSE_DCYCLE_LED0(void *a)
{
	uint8_t reg = *((uint8_t *)a);

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
	uint8_t reg = *((uint8_t *)a);

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
void app_read_REG_PULSE_FREQUENCY_LED0(void) {}
bool app_write_REG_PULSE_FREQUENCY_LED0(void *a)
{
	uint8_t reg = *((uint8_t *)a);

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
void app_read_REG_PULSE_FREQUENCY_LED1(void) {}
bool app_write_REG_PULSE_FREQUENCY_LED1(void *a)
{
	uint8_t reg = *((uint8_t *)a);

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
	uint16_t reg = *((uint16_t *)a);

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
	uint16_t reg = *((uint16_t *)a);

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
	uint8_t reg = *((uint8_t *)a);

	app_regs.REG_RAMP_CONFIG = reg;
	return true;
}

/************************************************************************/
/* REG_PROTOCOL0_DURATION                                               */
/************************************************************************/
void app_read_REG_PROTOCOL0_DURATION(void) {}
bool app_write_REG_PROTOCOL0_DURATION(void *a)
{
	uint16_t reg = *((uint16_t *)a);

	app_regs.REG_PROTOCOL0_DURATION = reg;
	return true;
}

/************************************************************************/
/* REG_PROTOCOL1_DURATION                                               */
/************************************************************************/
void app_read_REG_PROTOCOL1_DURATION(void) {}
bool app_write_REG_PROTOCOL1_DURATION(void *a)
{
	uint16_t reg = *((uint16_t *)a);

	app_regs.REG_PROTOCOL1_DURATION = reg;
	return true;
}

/************************************************************************/
/* REG_PROTOCOL0_DELAY                                                  */
/************************************************************************/
void app_read_REG_PROTOCOL0_DELAY(void) {}
bool app_write_REG_PROTOCOL0_DELAY(void *a)
{
	uint16_t reg = *((uint16_t *)a);

	app_regs.REG_PROTOCOL0_DELAY = reg;
	return true;
}

/************************************************************************/
/* REG_PROTOCOL1_DELAY                                                  */
/************************************************************************/
void app_read_REG_PROTOCOL1_DELAY(void) {}
bool app_write_REG_PROTOCOL1_DELAY(void *a)
{
	uint16_t reg = *((uint16_t *)a);

	app_regs.REG_PROTOCOL1_DELAY = reg;
	return true;
}

/************************************************************************/
/* REG_ENABLE_PROTOCOL                                                  */
/************************************************************************/
void app_read_REG_ENABLE_PROTOCOL(void) {}
bool app_write_REG_ENABLE_PROTOCOL(void *a)
{
    uint8_t reg = *((uint8_t *)a);
    if ((reg & B_LED0) && app_regs.REG_CHANNEL0_MODE == GM_PROTOCOL)
        channel_start_protocol(0);
    if ((reg & B_LED1) && app_regs.REG_CHANNEL1_MODE == GM_PROTOCOL)
        channel_start_protocol(1);
    app_regs.REG_ENABLE_PROTOCOL = reg;
    return true;
}

/************************************************************************/
/* REG_DISABLE_PROTOCOL                                                 */
/************************************************************************/
void app_read_REG_DISABLE_PROTOCOL(void) {}
bool app_write_REG_DISABLE_PROTOCOL(void *a)
{
    uint8_t reg = *((uint8_t *)a);
    if ((reg & B_LED0) && app_regs.REG_CHANNEL0_MODE == GM_PROTOCOL)
        channel_stop_protocol(0);
    if ((reg & B_LED1) && app_regs.REG_CHANNEL1_MODE == GM_PROTOCOL)
        channel_stop_protocol(1);
    app_regs.REG_DISABLE_PROTOCOL = reg;
    return true;
}

/************************************************************************/
/* REG_CHANNEL0_MODE                                                    */
/************************************************************************/
void app_read_REG_CHANNEL0_MODE(void) {}

bool app_write_REG_CHANNEL0_MODE(void *a)
{
    return channel_set_mode(0, *((uint8_t *)a));
}

/************************************************************************/
/* REG_CHANNEL1_MODE                                                    */
/************************************************************************/
void app_read_REG_CHANNEL1_MODE(void) {}

bool app_write_REG_CHANNEL1_MODE(void *a)
{
    return channel_set_mode(1, *((uint8_t *)a));
}

/************************************************************************/
/* REG_CHANNEL_STATE                                                    */
/************************************************************************/
void app_read_REG_CHANNEL_STATE(void) {}

bool app_write_REG_CHANNEL_STATE(void *a) { return false; }

/************************************************************************/
/* REG_RESERVED0                                                        */
/************************************************************************/
void app_read_REG_RESERVED0(void) {}
bool app_write_REG_RESERVED0(void *a)
{
	uint8_t reg = *((uint8_t *)a);

	app_regs.REG_RESERVED0 = reg;
	return true;
}

/************************************************************************/
/* REG_RESERVED1                                                        */
/************************************************************************/
void app_read_REG_RESERVED1(void) {}
bool app_write_REG_RESERVED1(void *a)
{
	uint8_t reg = *((uint8_t *)a);

	app_regs.REG_RESERVED1 = reg;
	return true;
}

/************************************************************************/
/* REG_RESERVED2                                                        */
/************************************************************************/
void app_read_REG_RESERVED2(void) {}
bool app_write_REG_RESERVED2(void *a)
{
	uint8_t reg = *((uint8_t *)a);

	app_regs.REG_RESERVED2 = reg;
	return true;
}

/************************************************************************/
/* REG_RESERVED3                                                        */
/************************************************************************/
void app_read_REG_RESERVED3(void) {}
bool app_write_REG_RESERVED3(void *a)
{
	uint8_t reg = *((uint8_t *)a);

	app_regs.REG_RESERVED3 = reg;
	return true;
}

/************************************************************************/
/* REG_RESERVED4                                                        */
/************************************************************************/
void app_read_REG_RESERVED4(void)
{
	// app_regs.REG_RESERVED4 = 0;
}

bool app_write_REG_RESERVED4(void *a)
{
	uint8_t reg = *((uint8_t *)a);

	app_regs.REG_RESERVED4 = reg;
	return true;
}

/************************************************************************/
/* REG_EVNT_ENABLE                                                      */
/************************************************************************/
void app_read_REG_EVNT_ENABLE(void) {}
bool app_write_REG_EVNT_ENABLE(void *a)
{
	uint8_t reg = *((uint8_t *)a);

	app_regs.REG_EVNT_ENABLE = reg;

	return true;
}
