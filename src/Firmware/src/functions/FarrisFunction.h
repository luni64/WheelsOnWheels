#pragma once

#include "ParametricFunction.h"
#include "FarrisWheel.h"


constexpr unsigned nrOfWheels = 4;

class FarrisFunction : public ParametricFunction
{
public:  
    Wheel wheels[nrOfWheels];
    
protected:
    inline void getCartesianCoords(float t, float *x, float *y);
};


// ------------------------------------------------------
// Inline Implementation
// -------------------------------------------------------

void FarrisFunction::getCartesianCoords(float t, float *x, float *y)
{
     *x = *y = 0.0f;

    for (unsigned i = 0; i < nrOfWheels; i++)
    {
        *x += wheels[i].x(t);
        *y += wheels[i].y(t);
    }    
}
