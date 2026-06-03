#ifndef _STRUCTS_H_
#define _STRUCTS_H

typedef enum {
    OFF, DELAY, RISE, ON, FALL
} ProtocolState;

typedef struct
{
    bool use_pulses;
    bool state;
    uint16_t time_on;
    uint16_t time_off;
    uint16_t countdown;
} Pulses;

typedef struct
{
    uint8_t use_ramps;
    uint16_t cycle_amount;
    uint16_t remainder_rise;
    uint16_t remainder_fall;
    uint16_t previous_value;
    uint16_t countdown_rise;
    uint16_t countdown_fall;
} Ramps;

typedef struct
{
    ProtocolState state;
    uint16_t delay;
    uint16_t duration;
    uint16_t target;
    bool has_duration;
    Pulses pulses;
    Ramps ramps;
} Protocol;


#endif /* _STRUCT_H_ */
