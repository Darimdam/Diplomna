// nrf24 Transmitter

#include <OneWire.h>

#include <SPI.h>
#include <RH_NRF24.h>

// Singleton instance of the radio driver
RH_NRF24 nrf24;

//temp sensors pin
OneWire  ds(2);

float values[4];
int j;

void setup() 
{
  Serial.begin(9600);
  j = 0;
  while (!Serial) 
    ; // wait for serial port to connect. Needed for Leonardo only
  if (!nrf24.init())
    Serial.println("init failed");
  // Defaults after init are 2.402 GHz (channel 2), 2Mbps, 0dBm
  if (!nrf24.setChannel(1))
    Serial.println("setChannel failed");
  if (!nrf24.setRF(RH_NRF24::DataRate2Mbps, RH_NRF24::TransmitPower0dBm))
    Serial.println("setRF failed");    
}


void loop()
{
  byte i;
  byte present = 0;
  byte type_s;
  byte data[9];
  byte addr[8];
  float celsius;

  if ( !ds.search(addr)) {
    sendData(values);
    
    j = 0;
    ds.reset_search();
    //delay(250);
    return;
  }

  // the first ROM byte indicates the chip
  switch (addr[0]) {
    case 0x10:
      //Serial.println("  Chip = DS18S20");  // or old DS1820
      type_s = 1;
      break;
    case 0x28:
      //Serial.println("  Chip = DS18B20");
      type_s = 0;
      break;
    case 0x22:
      //Serial.println("  Chip = DS1822");
      type_s = 0;
      break;
    default:
      //Serial.println("Device is not a DS18x20 family device.");
      return;
  }

  ds.reset();
  ds.select(addr);
  ds.write(0x44, 1);        // start conversion, with parasite power on at the end

  delay(500);     // maybe 750ms is enough, maybe not
  present = ds.reset();
  ds.select(addr);    
  ds.write(0xBE);         // Read Scratchpad


  for ( i = 0; i < 9; i++) 
  {           // we need 9 bytes
    data[i] = ds.read();

  }

    // Convert the data to actual temperature
    // because the result is a 16 bit signed integer, it should
    // be stored to an "int16_t" type, which is always 16 bits
    // even when compiled on a 32 bit processor.
    int16_t raw = (data[1] << 8) | data[0];
    if (type_s) {
    raw = raw << 3; // 9 bit resolution default
    if (data[7] == 0x10) {
      // "count remain" gives full 12 bit resolution
      raw = (raw & 0xFFF0) + 12 - data[6];
    }
    } else {
    byte cfg = (data[4] & 0x60);
    // at lower res, the low bits are undefined, so let's zero them
    if (cfg == 0x00) raw = raw & ~7;  // 9 bit resolution, 93.75 ms
    else if (cfg == 0x20) raw = raw & ~3; // 10 bit res, 187.5 ms
    else if (cfg == 0x40) raw = raw & ~1; // 11 bit res, 375 ms
    //// default is 12 bit resolution, 750 ms conversion time
  }
  celsius = (float)raw / 16.0;
  values[j] = celsius;
  j++;
}

void sendData(float dataRecieved[4])
{
  String outputData = String(dataRecieved[0]) + "|" + String(dataRecieved[1]) + "|" + String(dataRecieved[2]);

  uint8_t data[32];
  
  outputData.toCharArray(data, 31);
  Serial.println((char*)data);
  nrf24.send((uint8_t *)data, strlen(data));

  delay(400);
}