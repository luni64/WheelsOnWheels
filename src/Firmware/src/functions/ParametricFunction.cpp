#include "ParametricFunction.h"
#include <cmath>
#include <cfloat>

#include "Arduino.h"

float ParametricFunction::getFirst(float* r, float* phi, float _tStart, float _dt)
{
    dt = _dt;
    t = _tStart;

    getCartesianCoords(t, &x0, &y0);
    *r = r0 = sqrtf(x0 * x0 + y0 * y0);
    *phi = phi0 = (x0 > 0 || y0 < 0) ? atan2(y0, x0) : 0;

    return t;
}

void ParametricFunction::getInfo(float *r_min, float *r_max, float *t_min, float *t_max, float dt)
{
    float t = 0;
    *r_min = FLT_MAX;
    *r_max = *t_min = *t_max = 0; 
    
    while (t < 1.0f) // simply loop through complete function to find values
    {        
        float x, y;
        getCartesianCoords(t, &x, &y);

        float r = x * x + y * y;  // take sqrt at end
        if (r > *r_max)
        {
            *r_max = r;
            *t_max = t;
        }
        else if (r < *r_min)
        {
            *r_min = r;
            *t_min = t;
        }
        t += dt;
    }
    *r_min = sqrtf(*r_min);
    *r_max = sqrtf(*r_max);
}

float ParametricFunction::getNext(float *r, float *phi)
{
    float x1, y1;
    getCartesianCoords(t, &x1, &y1);

    *r = sqrtf(x1 * x1 + y1 * y1);
    if (r > 0 && r0 > 0)
    {
        phi0 += atan2f(x0 * y1 - y0 * x1, x0 * x1 + y0 * y1);
    }
    *phi = phi0;

    //Serial.printf("cart %f %f %f %f \n", x1, y1, *r, *phi);

    // housekeeping
    x0 = x1;
    y0 = y1;
    r0 = *r;
    t += dt;

    return t;
}
