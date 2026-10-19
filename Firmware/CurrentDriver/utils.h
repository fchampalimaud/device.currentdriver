#ifndef _STRUCTS_H_
#define _STRUCTS_H_

#include <util/atomic.h>

#include "app_ios_and_regs.h"

typedef enum
{
    WAVEFORM_LEVEL,
    WAVEFORM_PULSE
} WaveformKind;

typedef struct
{
    bool rise_enabled;
    bool fall_enabled;
    uint16_t duration_ms;
    bool rising;
    uint16_t step;
    uint32_t remainder;
    uint32_t error;
    uint32_t total_ticks;
    uint32_t remaining_ticks;
} RampState;

typedef struct
{
    bool on;
    uint16_t on_ticks;
    uint16_t off_ticks;
    uint16_t remaining_ticks;
} PulseState;

typedef struct
{
    WaveformKind kind;
    uint16_t applied_value;
    uint16_t target;
    union
    {
        RampState ramp;
        PulseState pulse;
    } waveform;
} OutputState;

typedef enum
{
    PROTOCOL_IDLE,
    PROTOCOL_DELAY,
    PROTOCOL_STARTING,
    PROTOCOL_HOLD,
    PROTOCOL_STOPPING
} ProtocolPhase;

typedef struct
{
    ProtocolPhase phase;
    uint32_t delay_ticks;
    uint32_t duration_ticks;
    bool has_duration;
} ProtocolState;

typedef struct
{
    OutputState output;
    ProtocolState protocol;
} ChannelState;

/* These entry points serialize state and DAC access against interrupts. */
bool channel_write_setpoint(uint8_t channel, float value, bool current);
void channel_reset(uint8_t channel);
void channel_apply_manual(uint8_t channel);
void channel_start_protocol(uint8_t channel);
void channel_stop_protocol(uint8_t channel);
bool channel_set_mode(uint8_t channel, uint8_t mode);
void channels_tick_500us(void);

#endif /* _STRUCTS_H_ */
