#include "hardware/hardware.h"
#include "communication/pcInterface.h"

void setup()
{
  hardware::begin();
  pcInterface::begin();

  Serial.println("Farris EggBot V0.1");
}

elapsedMillis sw;

void loop()
{
  pcInterface::tick();
  hardware::tick();

  if (hardware::eggBot.isRunning() && sw > 25)
  {
    sw = 0;
    float r, phi;
    hardware::eggBot.getPosition(&r, &phi);
    Serial.printf("%.4f %.4f\n", r, phi);
  }
}