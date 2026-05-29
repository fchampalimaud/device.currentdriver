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
// MISO                   Description: MISO
// DI1                    Description: Digital input 1

#define read_DI0 read_io(PORTB, 0)              // DI0
#define read_HOLD0 read_io(PORTD, 0)            // HOLD0
#define read_HOLD1 read_io(PORTD, 1)            // HOLD1
#define read_MISO read_io(PORTD, 6)             // MISO
#define read_DI1 read_io(PORTH, 0)              // DI1

/************************************************************************/
/* Definition of output pins                                            */
/************************************************************************/
// I_OFF_DAC0             Description: Disable DAC0 current
// I_OFF_DAC1             Description: Disable DAC1 current
// CE1                    Description: Chip enable 1 (active low)
// CE2                    Description: Chip enable 2 (active low)
// CS0                    Description: Chip select 0 (active low)
// CS1                    Description: Chip select 1 (active low)
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

/* CE1 */
#define set_CE1 set_io(PORTD, 3)
#define clr_CE1 clear_io(PORTD, 3)
#define tgl_CE1 toggle_io(PORTD, 3)
#define read_CE1 read_io(PORTD, 3)

/* CE2 */
#define set_CE2 set_io(PORTD, 4)
#define clr_CE2 clear_io(PORTD, 4)
#define tgl_CE2 toggle_io(PORTD, 4)
#define read_CE2 read_io(PORTD, 4)

/* CS0 */
#define set_CS0 set_io(PORTF, 5)
#define clr_CS0 clear_io(PORTF, 5)
#define tgl_CS0 toggle_io(PORTF, 5)
#define read_CS0 read_io(PORTF, 5)

/* CS1 */
#define set_CS1 set_io(PORTF, 6)
#define clr_CS1 clear_io(PORTF, 6)
#define tgl_CS1 toggle_io(PORTF, 6)
#define read_CS1 read_io(PORTF, 6)

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
	uint8_t REG_OUTPUTS_SET;
	uint8_t REG_OUTPUTS_CLEAR;
	uint8_t REG_OUTPUTS_TOGGLE;
	uint8_t REG_OUTPUTS_OUT;
	uint8_t REG_LED_ENABLE;
	uint8_t REG_LED_DISABLE;
	uint8_t REG_LED_OUT;
	uint8_t REG_LED_TARGET_STATE;
	float REG_LED0_CURRENT;
	float REG_LED1_CURRENT;
	float REG_LED0_MAX_CURRENT;
	float REG_LED1_MAX_CURRENT;
	float REG_DAC0_VOLTAGE;
	float REG_DAC1_VOLTAGE;
	uint8_t REG_PULSE_ENABLE;
	uint8_t REG_PULSE_DCYCLE_LED0;
	uint8_t REG_PULSE_DCYCLE_LED1;
	uint8_t REG_PULSE_FREQUENCY_LED0;
	uint8_t REG_PULSE_FREQUENCY_LED1;
	uint16_t REG_RAMP_LED0;
	uint16_t REG_RAMP_LED1;
	uint8_t REG_RAMP_CONFIG;
	uint16_t REG_PROTOCOL0_DURATION;
	uint16_t REG_PROTOCOL1_DURATION;
	uint16_t REG_PROTOCOL0_DELAY;
	uint16_t REG_PROTOCOL1_DELAY;
	uint8_t REG_ENABLE_PROTOCOL;
	uint8_t REG_DISABLE_PROTOCOL;
	uint8_t REG_DI0_TRIGGER;
	uint8_t REG_DI1_TRIGGER;
	uint8_t REG_RESERVED0;
	uint8_t REG_RESERVED1;
	uint8_t REG_RESERVED2;
	uint8_t REG_RESERVED3;
	uint8_t REG_EVNT_ENABLE;
} AppRegs;

/************************************************************************/
/* Registers' address                                                   */
/************************************************************************/
/* Registers */
#define ADD_REG_PORT_DIS                    32 // U8     Reflects the state of DI digital lines of each Port
#define ADD_REG_OUTPUTS_SET                 33 // U8     Set the correspondent output
#define ADD_REG_OUTPUTS_CLEAR               34 // U8     Clear the correspondent output
#define ADD_REG_OUTPUTS_TOGGLE              35 // U8     Toggle the correspondent output
#define ADD_REG_OUTPUTS_OUT                 36 // U8     Control the correspondent output
#define ADD_REG_LED_ENABLE                  37 // U8     Enable driver on the selected output
#define ADD_REG_LED_DISABLE                 38 // U8     Disable driver on the selected output
#define ADD_REG_LED_OUT                     39 // U8     Control the correspondent LED output
#define ADD_REG_LED_TARGET_STATE            40 // U8     Sends an event when the LED reaches the target value
#define ADD_REG_LED0_CURRENT                41 // FLOAT  Configuration of current to drive LED 0 [0:1000] mA
#define ADD_REG_LED1_CURRENT                42 // FLOAT  Configuration of current to drive LED 1 [0:1000] mA
#define ADD_REG_LED0_MAX_CURRENT            43 // FLOAT  Configuration of current to drive LED 0 [0:1000] mA
#define ADD_REG_LED1_MAX_CURRENT            44 // FLOAT  Configuration of current to drive LED 1 [0:1000] mA
#define ADD_REG_DAC0_VOLTAGE                45 // FLOAT  Configuration of DAC 0 voltage [0:5000] mV
#define ADD_REG_DAC1_VOLTAGE                46 // FLOAT  Configuration of DAC 1 voltage [0:5000] mV
#define ADD_REG_PULSE_ENABLE                47 // U8     Enables the pulse function for the specified output DACs/LEDs.
#define ADD_REG_PULSE_DCYCLE_LED0           48 // U8     Specifies the duty cycle of the output pulse from 1 to 100.
#define ADD_REG_PULSE_DCYCLE_LED1           49 // U8     Specifies the duty cycle of the output pulse from 1 to 100.
#define ADD_REG_PULSE_FREQUENCY_LED0        50 // U8     Specifies the frequency of the output pulse in Hz.
#define ADD_REG_PULSE_FREQUENCY_LED1        51 // U8     Specifies the frequency of the output pulse in Hz.
#define ADD_REG_RAMP_LED0                   52 // U16    Specifies the ramp time of the transitions between different current/voltage values in milliseconds. The ramp will only work if the pulse function is off.
#define ADD_REG_RAMP_LED1                   53 // U16    Specifies the ramp time of the transitions between different current/voltage values in milliseconds. The ramp will only work if the pulse function is off.
#define ADD_REG_RAMP_CONFIG                 54 // U8     Specifies when the ramps are applied for each DAC/LED.
#define ADD_REG_PROTOCOL0_DURATION          55 // U16    Specifies the duration of LED0 protocol
#define ADD_REG_PROTOCOL1_DURATION          56 // U16    Specifies the duration of LED1 protocol
#define ADD_REG_PROTOCOL0_DELAY             57 // U16    Specifies the delay of the LED0 protocol
#define ADD_REG_PROTOCOL1_DELAY             58 // U16    Specifies the delay of the LED1 protocol
#define ADD_REG_ENABLE_PROTOCOL             59 // U8     Enable the correspondent protocol
#define ADD_REG_DISABLE_PROTOCOL            60 // U8     Disables the correspondent protocol
#define ADD_REG_DI0_TRIGGER                 61 // U8     Configures the callback function triggered when DI0 is triggered
#define ADD_REG_DI1_TRIGGER                 62 // U8     Configures the callback function triggered when DI1 is triggered
#define ADD_REG_RESERVED0                   63 // U8     Reserved for future use
#define ADD_REG_RESERVED1                   64 // U8     Reserved for future use
#define ADD_REG_RESERVED2                   65 // U8     Reserved for future use
#define ADD_REG_RESERVED3                   66 // U8     Reserved for future use
#define ADD_REG_EVNT_ENABLE                 67 // U8     Enable the Events

/************************************************************************/
/* PWM Generator registers' memory limits                               */
/*                                                                      */
/* DON'T change the APP_REGS_ADD_MIN value !!!                          */
/* DON'T change these names !!!                                         */
/************************************************************************/
/* Memory limits */
#define APP_REGS_ADD_MIN                    0x20
#define APP_REGS_ADD_MAX                    0x43
#define APP_NBYTES_OF_REG_BANK              60

/************************************************************************/
/* Registers' bits                                                      */
/************************************************************************/
#define B_DI0                              (1<<0)       // Digital input 0
#define B_DI1                              (1<<1)       // Digital input 1
#define B_DO0                              (1<<0)       // Digital output 0
#define B_DO1                              (1<<1)       // Digital output 1
#define B_LED0                             (1<<0)       // 
#define B_LED1                             (1<<1)       // 
#define B_LED0_RISE                        (1<<0)       // 
#define B_LED0_FALL                        (1<<1)       // 
#define B_LED1_RISE                        (1<<2)       // 
#define B_LED1_FALL                        (1<<3)       // 
#define GM_DIGITAL                         (0<<0)       // Used as pure digital input
#define GM_CONTROL_LED                     (1<<0)       // Controls respective DAC based on digital input state
#define GM_START_PROTOCOL                  (2<<0)       // Starts the specified protocol when rising edge
#define GM_START_AND_STOP_PROTOCOL         (3<<0)       // Starts the specified protocol when rising edge and stops when falling edge
#define B_EVT_PORT_DIS                     (1<<0)       // Event of register DIs
#define B_EVT_LED_STATE                    (1<<1)       // Event of the state of the LEDs

#endif /* _APP_REGS_H_ */