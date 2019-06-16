#pragma once

#include "TeensyStep.h"

class EggBot
{
public:
    EggBot(Stepper &spindle, Stepper &arm);
    void setArmScale(float s);
    void setSpindleScale(float s);
    void begin();
    void end();

    void setPosition(float r, float phi);
    void getPosition(float* r, float* phi);
    void moveTo(float r, float phi);
    bool isRunning();

protected:
    float p;

    Stepper &spindle, &arm;
    RotateControl spindleController, armController;
    float armScale, spindleScale;
};