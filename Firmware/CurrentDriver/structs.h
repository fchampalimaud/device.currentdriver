#ifndef _STRUCTS_H_
#define _STRUCTS_H

typedef struct
{
    uint16_t dac0, dac1;
    uint16_t ramp_dac0, ramp_dac1;
} countdown_t;

typedef struct
{
    uint8_t prescaler_dac0, prescaler_dac1;
    uint8_t prescaler_do0;
    uint16_t target_dac0, target_dac1;
    uint16_t target_do0;
    uint16_t dcycle_dac0, dcycle_dac1;
    uint16_t dcycle_do0;
} timer_conf_t;

typedef struct
{
    bool pwm_dac0, pwm_dac1;
} is_new_timer_conf_t;

typedef struct
{
    uint16_t pwm_on_dac0, pwm_on_dac1;
    uint16_t pwm_off_dac0, pwm_off_dac1;
    bool is_on_dac0, is_on_dac1;
} pulse_timings;

/* State of output ports */
typedef struct
{
    bool dac0, dac1;
    bool do0;
} pwm_possibilities_t;

typedef struct 
{
    bool is_increasing_dac0, is_increasing_dac1;
    uint16_t cycle_amount_dac0, cycle_amount_dac1;
    uint16_t previous_value_dac0, previous_value_dac1;
    uint16_t intended_value_dac0, intended_value_dac1;
} ramp_info;

#endif /* _STRUCT_H_ */