
#pragma once

class ParametricFunction
{
public:
    // Calculates radius and angle for a given parameter t.
    // Make sure to use consecutive values of t. Angle between two calls to the function must not be larget than +/- PI
    
    float getFirst(float *r, float* phi, float tStart = 0, float dt = 0.001f);
    float getNext(float *r, float *phi);

    void getInfo(float* r_min, float* r_max, float* t_min, float* t_max, float dt );

protected:
    virtual void getCartesianCoords(float t, float *x, float *y) = 0;
    float x0, y0;
    float phi0, r0; // last values of cart. coordinates

    float t, dt;
};