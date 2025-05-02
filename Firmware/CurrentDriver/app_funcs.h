#ifndef _APP_FUNCTIONS_H_
#define _APP_FUNCTIONS_H_
#include <avr/io.h>


/************************************************************************/
/* Define if not defined                                                */
/************************************************************************/
#ifndef bool
	#define bool uint8_t
#endif
#ifndef true
	#define true 1
#endif
#ifndef false
	#define false 0
#endif


/************************************************************************/
/* Prototypes                                                           */
/************************************************************************/
void app_read_REG_PORT_DIS(void);
void app_read_REG_OUTPUTS_SET(void);
void app_read_REG_OUTPUTS_CLEAR(void);
void app_read_REG_OUTPUTS_TOGGLE(void);
void app_read_REG_OUTPUTS_OUT(void);
void app_read_REG_LED0_CURRENT(void);
void app_read_REG_LED1_CURRENT(void);
void app_read_REG_DAC0_VOLTAGE(void);
void app_read_REG_DAC1_VOLTAGE(void);
void app_read_REG_LED_ENABLE(void);
void app_read_REG_LED_DISABLE(void);
void app_read_REG_LED_OUT(void);
void app_read_REG_LED0_MAX_CURRENT(void);
void app_read_REG_LED1_MAX_CURRENT(void);
void app_read_REG_PULSE_ENABLE(void);
void app_read_REG_PULSE_DURATION_LED0(void);
void app_read_REG_PULSE_DURATION_LED1(void);
void app_read_REG_PULSE_FREQUENCY_LED0(void);
void app_read_REG_PULSE_FREQUENCY_LED1(void);
void app_read_REG_RAMP_LED0(void);
void app_read_REG_RAMP_LED1(void);
void app_read_REG_RESERVED0(void);
void app_read_REG_RESERVED1(void);
void app_read_REG_RESERVED2(void);
void app_read_REG_RESERVED3(void);
void app_read_REG_RESERVED4(void);
void app_read_REG_EVNT_ENABLE(void);

bool app_write_REG_PORT_DIS(void *a);
bool app_write_REG_OUTPUTS_SET(void *a);
bool app_write_REG_OUTPUTS_CLEAR(void *a);
bool app_write_REG_OUTPUTS_TOGGLE(void *a);
bool app_write_REG_OUTPUTS_OUT(void *a);
bool app_write_REG_LED0_CURRENT(void *a);
bool app_write_REG_LED1_CURRENT(void *a);
bool app_write_REG_DAC0_VOLTAGE(void *a);
bool app_write_REG_DAC1_VOLTAGE(void *a);
bool app_write_REG_LED_ENABLE(void *a);
bool app_write_REG_LED_DISABLE(void *a);
bool app_write_REG_LED_OUT(void *a);
bool app_write_REG_LED0_MAX_CURRENT(void *a);
bool app_write_REG_LED1_MAX_CURRENT(void *a);
bool app_write_REG_PULSE_ENABLE(void *a);
bool app_write_REG_PULSE_DURATION_LED0(void *a);
bool app_write_REG_PULSE_DURATION_LED1(void *a);
bool app_write_REG_PULSE_FREQUENCY_LED0(void *a);
bool app_write_REG_PULSE_FREQUENCY_LED1(void *a);
bool app_write_REG_RAMP_LED0(void *a);
bool app_write_REG_RAMP_LED1(void *a);
bool app_write_REG_RESERVED0(void *a);
bool app_write_REG_RESERVED1(void *a);
bool app_write_REG_RESERVED2(void *a);
bool app_write_REG_RESERVED3(void *a);
bool app_write_REG_RESERVED4(void *a);
bool app_write_REG_EVNT_ENABLE(void *a);


#endif /* _APP_FUNCTIONS_H_ */