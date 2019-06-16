#pragma once

#include <cmath>
#include "wiring.h"

class Wheel
{
public:
    void setParams(float r, float f, float phi0);

    inline float x(float t);
    inline float y(float t);

protected:
    float f = 0.0f;
    float r = 0.0f;
    float phi0 = 0.0f;
};


// inline implementation

void Wheel::setParams(float _f, float _r, float _phi0)
{
    r = _r;
    f = _f;
    phi0 = _phi0;
}

float Wheel::x(float t)
{
    return r * cosf(TWO_PI * f * t + phi0);
}

float Wheel::y(float t)
{
    return r * sinf(TWO_PI * f * t + phi0);
}

