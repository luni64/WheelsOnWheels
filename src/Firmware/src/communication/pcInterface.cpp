#include "pcInterface.h"
#include "SerialCommand.h"
#include "../hardware/hardware.h"
#include "../EggController.h"
#include "../functions/FarrisFunction.h"

namespace pcInterface
{

SerialCommand sCmd;

FarrisFunction f;

void setParams()
{
    char *arg;

    float params[12];

    unsigned i;
    for (i = 0; i < 12; i++)
    {
        arg = sCmd.next(); // Get the next argument from the SerialCommand object buffer
        if (arg == nullptr)
            break;

        params[i] = strtof(arg, nullptr);
        Serial.print(params[i]);
        Serial.print(" ");
    }

    if (i != 12)
    {
        Serial.println("ERR");
        return;
    }
    Serial.println();

    f.wheels[0].setParams(params[0], params[1], params[2]);
    f.wheels[1].setParams(params[3], params[4], params[5]);
    f.wheels[2].setParams(params[6], params[7], params[8]);
    f.wheels[3].setParams(params[9], params[10], params[11]);
}

void startBot()
{
    hardware::eggController.stopDrawing();
    hardware::eggController.drawFunction(&f, 1.2, 35000);
}

void stopBot()
{
    hardware::eggController.stopDrawing();
}

void begin()
{
    sCmd.addCommand("start", startBot);
    sCmd.addCommand("stop", stopBot);
    sCmd.addCommand("farris", setParams);

    f.wheels[0].setParams(-2, 1.00, 0);
    f.wheels[1].setParams(5, 0.50, 0);
    f.wheels[2].setParams(19, 0.25, 0);
    f.wheels[3].setParams(0, 0, 0);
}

void tick()
{
    sCmd.readSerial();
}

} // namespace pcInterface
