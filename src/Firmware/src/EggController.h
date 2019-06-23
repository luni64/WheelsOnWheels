#pragma once

#include "functions/ParametricFunction.h"
#include "hardware/EggBot.h"
#include "elapsedMillis.h"

class EggController
{
public:
    inline EggController(EggBot &eggBot);

    inline void drawFunction(ParametricFunction *f, float size = 1.0f, unsigned samplePeriod = 35000);
    inline void stopDrawing();

    inline void tick();

protected:
    EggBot &eggBot;
    ParametricFunction *f = nullptr;
    elapsedMicros sampleTimer;
    unsigned samplePeriod;

    float rStpFac;
};

EggController::EggController(EggBot &_eggBot)
    : eggBot(_eggBot)
{
}

void EggController::drawFunction(ParametricFunction *func, float size, unsigned _samplePeriod)
{
    samplePeriod = _samplePeriod;

    // get min and max values of the function
    float r_min, r_max, t_min, t_max;
    func->getInfo(&r_min, &r_max, &t_min, &t_max, 0.001);

    //  Serial.printf("%f, %f, %f, %f", r_min, r_max, t_min, t_max);
    //  return;

    // setup function and bot to start at smallest r
    float r, phi;
    func->getFirst(&r, &phi, t_min, 0.001);
    eggBot.setArmScale( 1000 * size / r_max);
    eggBot.setPosition(r,phi);

    // start
    eggBot.begin();
    sampleTimer = 0;
    f = func;
}

void EggController::stopDrawing()
{
    f = nullptr;
    eggBot.end();
}

void EggController::tick()
{
    if (sampleTimer < samplePeriod || f == nullptr)
        return;
    sampleTimer = 0;

    float r, phi;
    f->getNext(&r, &phi);
    eggBot.moveTo(r, phi);
}