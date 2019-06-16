#include "EggBot.h"
#include <cmath>

EggBot::EggBot(Stepper &_spindle, Stepper &_arm)
    : spindle(_spindle), arm(_arm), spindleController(5,1000)
{
    p = 1;

    spindle.setMaxSpeed(10000);
    spindle.setAcceleration(200000);

    arm.setMaxSpeed(50000);
    arm.setAcceleration(200000);

}

void EggBot::setArmScale(float s)
{
    armScale = s;
}

void EggBot::setSpindleScale(float s)
{
    spindleScale = s;
}

void EggBot::begin()
{
    spindleController.rotateAsync(spindle); // let the spindle run with constant speed
    spindleController.overrideSpeed(0);     // start with stopped spindle

    armController.rotateAsync(arm);
    armController.overrideSpeed(0); // start with stopped slide
}

void EggBot::end()
{
    spindleController.stop();
    armController.stop();

    StepControl ctrl;
    arm.setTargetAbs(0);
    ctrl.move(0.1,arm);
}

void EggBot::setPosition(float r, float phi)
{
    spindle.setPosition(phi * (3200.0f / TWO_PI));
    arm.setPosition(r * armScale);
}

void EggBot::getPosition(float* r, float* phi)
{
    *r = arm.getPosition() / armScale;
    *phi = spindle.getPosition() * (TWO_PI / 3200.0f);
}

bool EggBot::isRunning()
{
    return spindleController.isRunning() || armController.isRunning();
}

void EggBot::moveTo(float r, float phi)
{
    float rCur, phiCur;
    getPosition(&rCur, &phiCur);

    // float rCur = arm.getPosition() / armScale;
    // float phiCur = spindle.getPosition() * (TWO_PI / 3200.0);

    spindleController.overrideSpeed(2.0f * (phi - phiCur));
    armController.overrideSpeed(0.5 * (r - rCur));

    //Serial.printf("%.5f\t%.5f\t%.5f\t%.5f\n", r, phi, rCur, phiCur);
}