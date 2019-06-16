#pragma once

#include "../EggController.h"

namespace hardware
{
    extern void begin();
    extern void tick();

    extern EggController eggController;
    extern EggBot eggBot;
}