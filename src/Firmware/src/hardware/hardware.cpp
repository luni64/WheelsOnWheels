#include "hardware.h" 
#include "EggBot.h"
#include "TeensyStep.h"

namespace hardware
{
    Stepper arm(0,1);
    Stepper spindle(2,3);

    EggBot eggBot(spindle, arm);
    EggController eggController(eggBot);

    
    // setup hardware and peripherals
    void begin()
    {
        pinMode(LED_BUILTIN, OUTPUT);        
        while (!Serial && millis() < 500){}
    }


    // perform all periodic hardware tasks; call as often as possible
    void tick()
    {
        eggController.tick();

        // heartbeat
        static elapsedMillis blinkTimer = 0; 
        if (blinkTimer > 250)
        {
            blinkTimer = 0;
           // digitalWriteFast(LED_BUILTIN, !digitalReadFast(LED_BUILTIN));
        }
    }
} 