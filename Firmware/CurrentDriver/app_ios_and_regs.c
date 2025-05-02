#include <avr/io.h>
#include "hwbp_core_types.h"
#include "app_ios_and_regs.h"

/************************************************************************/
/* Configure and initialize IOs                                         */
/************************************************************************/
void init_ios(void)
{	/* Configure input pins */
	io_pin2in(&PORTB, 0, PULL_IO_UP, SENSE_IO_EDGES_BOTH);               // DI0
	io_pin2in(&PORTD, 0, PULL_IO_UP, SENSE_IO_EDGES_BOTH);               // HOLD0
	io_pin2in(&PORTD, 1, PULL_IO_UP, SENSE_IO_EDGES_BOTH);               // HOLD1
	io_pin2in(&PORTD, 6, PULL_IO_UP, SENSE_IO_EDGES_BOTH);               // MISO
	io_pin2in(&PORTH, 0, PULL_IO_UP, SENSE_IO_EDGES_BOTH);               // DI1

	/* Configure input interrupts */
	io_set_int(&PORTB, INT_LEVEL_LOW, 0, (1<<0), false);                 // DI0
	io_set_int(&PORTD, INT_LEVEL_LOW, 0, (1<<0), false);                 // HOLD0
	io_set_int(&PORTD, INT_LEVEL_LOW, 0, (1<<1), false);                 // HOLD1
	io_set_int(&PORTH, INT_LEVEL_LOW, 0, (1<<0), false);                 // DI1

	/* Configure output pins */
	io_pin2out(&PORTB, 5, OUT_IO_DIGITAL, IN_EN_IO_EN);                  // I_OFF_DAC0
	io_pin2out(&PORTB, 6, OUT_IO_DIGITAL, IN_EN_IO_EN);                  // I_OFF_DAC1
	io_pin2out(&PORTD, 3, OUT_IO_DIGITAL, IN_EN_IO_DIS);                 // CE1
	io_pin2out(&PORTD, 4, OUT_IO_DIGITAL, IN_EN_IO_DIS);                 // CE2
	io_pin2out(&PORTF, 5, OUT_IO_DIGITAL, IN_EN_IO_DIS);                 // CS0
	io_pin2out(&PORTF, 6, OUT_IO_DIGITAL, IN_EN_IO_DIS);                 // CS1
	io_pin2out(&PORTD, 5, OUT_IO_DIGITAL, IN_EN_IO_DIS);                 // MOSI
	io_pin2out(&PORTD, 7, OUT_IO_DIGITAL, IN_EN_IO_DIS);                 // SCK
	io_pin2out(&PORTE, 0, OUT_IO_DIGITAL, IN_EN_IO_EN);                  // DO1
	io_pin2out(&PORTF, 0, OUT_IO_DIGITAL, IN_EN_IO_EN);                  // DO0

	/* Initialize output pins */
	clr_I_OFF_DAC0;
	clr_I_OFF_DAC1;
	set_CE1;
	set_CE2;
	set_CS0;
	set_CS1;
	clr_MOSI;
	clr_SCK;
	clr_DO1;
	clr_DO0;
}

/************************************************************************/
/* Registers' stuff                                                     */
/************************************************************************/
AppRegs app_regs;

uint8_t app_regs_type[] = {
	TYPE_U8,
	TYPE_U16,
	TYPE_U16,
	TYPE_U16,
	TYPE_U16,
	TYPE_FLOAT,
	TYPE_FLOAT,
	TYPE_FLOAT,
	TYPE_FLOAT,
	TYPE_U8,
	TYPE_U8,
	TYPE_U8,
	TYPE_FLOAT,
	TYPE_FLOAT,
	TYPE_U8,
	TYPE_U8,
	TYPE_U8,
	TYPE_U8,
	TYPE_U8,
	TYPE_U16,
	TYPE_U16,
	TYPE_U8,
	TYPE_U8,
	TYPE_U8,
	TYPE_U8,
	TYPE_U8,
	TYPE_U8
};

uint16_t app_regs_n_elements[] = {
	1,
	1,
	1,
	1,
	1,
	1,
	1,
	1,
	1,
	1,
	1,
	1,
	1,
	1,
	1,
	1,
	1,
	1,
	1,
	1,
	1,
	1,
	1,
	1,
	1,
	1,
	1
};

uint8_t *app_regs_pointer[] = {
	(uint8_t*)(&app_regs.REG_PORT_DIS),
	(uint8_t*)(&app_regs.REG_OUTPUTS_SET),
	(uint8_t*)(&app_regs.REG_OUTPUTS_CLEAR),
	(uint8_t*)(&app_regs.REG_OUTPUTS_TOGGLE),
	(uint8_t*)(&app_regs.REG_OUTPUTS_OUT),
	(uint8_t*)(&app_regs.REG_LED0_CURRENT),
	(uint8_t*)(&app_regs.REG_LED1_CURRENT),
	(uint8_t*)(&app_regs.REG_DAC0_VOLTAGE),
	(uint8_t*)(&app_regs.REG_DAC1_VOLTAGE),
	(uint8_t*)(&app_regs.REG_LED_ENABLE),
	(uint8_t*)(&app_regs.REG_LED_DISABLE),
	(uint8_t*)(&app_regs.REG_LED_OUT),
	(uint8_t*)(&app_regs.REG_LED0_MAX_CURRENT),
	(uint8_t*)(&app_regs.REG_LED1_MAX_CURRENT),
	(uint8_t*)(&app_regs.REG_PULSE_ENABLE),
	(uint8_t*)(&app_regs.REG_PULSE_DURATION_LED0),
	(uint8_t*)(&app_regs.REG_PULSE_DURATION_LED1),
	(uint8_t*)(&app_regs.REG_PULSE_FREQUENCY_LED0),
	(uint8_t*)(&app_regs.REG_PULSE_FREQUENCY_LED1),
	(uint8_t*)(&app_regs.REG_RAMP_LED0),
	(uint8_t*)(&app_regs.REG_RAMP_LED1),
	(uint8_t*)(&app_regs.REG_RESERVED0),
	(uint8_t*)(&app_regs.REG_RESERVED1),
	(uint8_t*)(&app_regs.REG_RESERVED2),
	(uint8_t*)(&app_regs.REG_RESERVED3),
	(uint8_t*)(&app_regs.REG_RESERVED4),
	(uint8_t*)(&app_regs.REG_EVNT_ENABLE)
};