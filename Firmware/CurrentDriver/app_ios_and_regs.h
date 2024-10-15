#ifndef _APP_IOS_AND_REGS_H_
#define _APP_IOS_AND_REGS_H_
#include "cpu.h"

void init_ios(void);
/************************************************************************/
/* Definition of input pins                                             */
/************************************************************************/
// DI0                    Description: Digital input 0
// HOLD0                  Description: Memory 0 hold (active low)
// HOLD1                  Description: Memory 1 hold (active low)
// CE1                    Description: Chip enable 1 (active low)
// CE2                    Description: Chip enable 2 (active low)
// MISO                   Description: MISO
// DI1                    Description: Digital input 1

#define read_DI0 read_io(PORTB, 0)              // DI0
#define read_HOLD0 read_io(PORTD, 0)            // HOLD0
#define read_HOLD1 read_io(PORTD, 1)            // HOLD1
#define read_CE1 read_io(PORTD, 3)              // CE1
#define read_CE2 read_io(PORTD, 4)              // CE2
#define read_MISO read_io(PORTD, 6)             // MISO
#define read_DI1 read_io(PORTH, 0)              // DI1

/************************************************************************/
/* Definition of output pins                                            */
/************************************************************************/
// I_OFF_DAC0             Description: Disable DAC0 current
// I_OFF_DAC1             Description: Disable DAC1 current
// MOSI                   Description: MOSI
// SCK                    Description: SCK
// DO1                    Description: Output DO1
// DO0                    Description: Output DO0

/* I_OFF_DAC0 */
#define set_I_OFF_DAC0 set_io(PORTB, 5)
#define clr_I_OFF_DAC0 clear_io(PORTB, 5)
#define tgl_I_OFF_DAC0 toggle_io(PORTB, 5)
#define read_I_OFF_DAC0 read_io(PORTB, 5)

/* I_OFF_DAC1 */
#define set_I_OFF_DAC1 set_io(PORTB, 6)
#define clr_I_OFF_DAC1 clear_io(PORTB, 6)
#define tgl_I_OFF_DAC1 toggle_io(PORTB, 6)
#define read_I_OFF_DAC1 read_io(PORTB, 6)

/* MOSI */
#define set_MOSI set_io(PORTD, 5)
#define clr_MOSI clear_io(PORTD, 5)
#define tgl_MOSI toggle_io(PORTD, 5)
#define read_MOSI read_io(PORTD, 5)

/* SCK */
#define set_SCK set_io(PORTD, 7)
#define clr_SCK clear_io(PORTD, 7)
#define tgl_SCK toggle_io(PORTD, 7)
#define read_SCK read_io(PORTD, 7)

/* DO1 */
#define set_DO1 set_io(PORTE, 0)
#define clr_DO1 clear_io(PORTE, 0)
#define tgl_DO1 toggle_io(PORTE, 0)
#define read_DO1 read_io(PORTE, 0)

/* DO0 */
#define set_DO0 set_io(PORTF, 0)
#define clr_DO0 clear_io(PORTF, 0)
#define tgl_DO0 toggle_io(PORTF, 0)
#define read_DO0 read_io(PORTF, 0)


/************************************************************************/
/* Registers' structure                                                 */
/************************************************************************/
typedef struct
{
	uint8_t REG_PORT_DIS;
	uint16_t REG_OUTPUTS_SET;
	uint16_t REG_OUTPUTS_CLEAR;
	uint16_t REG_OUTPUTS_TOGGLE;
	uint16_t REG_OUTPUTS_OUT;
	float REG_LED0_CURRENT;
	float REG_LED1_CURRENT;
	float REG_DAC0_VOLTAGE;
	float REG_DAC1_VOLTAGE;
	uint8_t REG_LED_ENABLE;
	uint8_t REG_LED_DISABLE;
	uint8_t REG_LED_OUT;
	float REG_LED0_MAX_CURRENT;
	float REG_LED1_MAX_CURRENT;
	uint8_t REG_RESERVED0;
	uint8_t REG_RESERVED1;
	uint8_t REG_RESERVED2;
	uint8_t REG_RESERVED3;
	uint8_t REG_RESERVED4;
	uint8_t REG_EVNT_ENABLE;
} AppRegs;

/************************************************************************/
/* Registers' address                                                   */
/************************************************************************/
/* Registers */
#define ADD_REG_PORT_DIS                    32 // U8     Reflects the state of DI digital lines of each Port
#define ADD_REG_OUTPUTS_SET                 33 // U16    Set the correspondent output
#define ADD_REG_OUTPUTS_CLEAR               34 // U16    Clear the correspondent output
#define ADD_REG_OUTPUTS_TOGGLE              35 // U16    Toggle the correspondent output
#define ADD_REG_OUTPUTS_OUT                 36 // U16    Control the correspondent output
#define ADD_REG_LED0_CURRENT                37 // Float  Configuration of current to drive LED 0 [0:1000] mA
#define ADD_REG_LED1_CURRENT                38 // Float  Configuration of current to drive LED 1 [0:1000] mA
#define ADD_REG_DAC0_VOLTAGE                39 // Float  Configuration of DAC 0 voltage [0:4000] mV
#define ADD_REG_DAC1_VOLTAGE                40 // Float  Configuration of DAC 1 voltage [0:4000] mV
#define ADD_REG_LED_ENABLE                  41 // U8     Enable driver on the selected output
#define ADD_REG_LED_DISABLE                 42 // U8     Disable driver on the selected output
#define ADD_REG_LED_OUT                     43 // U8     Control the correspondent output
#define ADD_REG_LED0_MAX_CURRENT            44 // Float  Configuration of current to drive LED 0 [0:1000] mA
#define ADD_REG_LED1_MAX_CURRENT            45 // Float  Configuration of current to drive LED 1 [0:1000] mA
#define ADD_REG_RESERVED0                   46 // U8     Reserved for future use
#define ADD_REG_RESERVED1                   47 // U8     Reserved for future use
#define ADD_REG_RESERVED2                   48 // U8     Reserved for future use
#define ADD_REG_RESERVED3                   49 // U8     Reserved for future use
#define ADD_REG_RESERVED4                   50 // U8     Reserved for future use
#define ADD_REG_EVNT_ENABLE                 51 // U8     Enable the Events

/************************************************************************/
/* Current Driver registers' memory limits                              */
/*                                                                      */
/* DON'T change the APP_REGS_ADD_MIN value !!!                          */
/* DON'T change these names !!!                                         */
/************************************************************************/
/* Memory limits */
#define APP_REGS_ADD_MIN                    0x20
#define APP_REGS_ADD_MAX                    0x33
#define APP_NBYTES_OF_REG_BANK              20

/************************************************************************/
/* Registers' bits                                                      */
/************************************************************************/
#define B_DI0                              (1<<0)       // Digital input 0
#define B_DI1                              (1<<1)       // Digital input 1
#define B_DO0                              (1<<0)       // Digital output 0
#define B_DO1                              (1<<1)       // Digital output 1
#define B_LED0                             (1<<0)       // 
#define B_LED1                             (1<<1)       // 
#define B_EVT_PORT_DIS                     (1<<0)       // Event of register DIs

#endif /* _APP_REGS_H_ */