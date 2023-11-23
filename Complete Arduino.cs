/*CESA Weather Station 2019 */

//include relevant libraries for the Arduino, sensor and Real time clock (RTC)
#include "DHT.h"
#include "DHT_U.h"
#include "SD.h"
#include "Wire.h"
#include "RTClib.h" 
#include <LiquidCrystal_I2C.h>


//define DHT pin
#define DHTPIN 3        //  pin the sensor is connected to on arduino (can be any number from 2 - 13)
#define DHTTYPE DHT22   // DHT22 is the type of temperature sensor we are using  


File myFile;
LiquidCrystal_I2C lcd(0x27, 16, 2);
// initialize DHT sensor for normal 16mhz Arduino
DHT dht(DHTPIN, DHTTYPE);
RTC_DS1307 RTC;


void setup () {   // setup function runs once

  Serial.begin(9600); //opens serial connection, sets the data rate to 9600 bps (bitspersecond) and waits for the port to open
  lcd.init();
  lcd.backlight();
  lcd.begin(20, 4);
  
  while (!Serial) {
    ; // wait for serial port to connect. Needed for native USB port only
  }

  //pinMode(speakerPin, OUTPUT);

  Serial.print("Initializing SD card...");

  if (!SD.begin(4)) {
    Serial.println("initialization failed!");
    while (1);
  }
  Serial.println("initialization done.");
  

  // creates a new data file if one does not already exist
  if (not SD.exists("DATA.txt")) {
    myFile = SD.open("DATA.txt", FILE_WRITE);
    myFile.print("DATE, TIME, TEMP *C, HUMIDITY");
    myFile.println();
    myFile.close();
  }
  
  //initializing the DHT and RTC
  dht.begin();
  Wire.begin();
  RTC.begin();

  // Check to see if the RTC is keeping time.  
  if (! RTC.isrunning()) {
    Serial.println("RTC is NOT running!");
    // Sets the RTC to the time that this sketch was compiled 
    RTC.adjust(DateTime(F(__DATE__), F(__TIME__)));
  }

  

}


void loop () {
  DateTime now = RTC.now(); //query the RTC for the current time

  //read and store the temperature and humidity
  float t = dht.readTemperature();
  float h = dht.readHumidity();
  
  

  if  (isnan(t)) {
    Serial.println("Failed to read from DHT sensor!");
    return;
  }

  if (20<t<30) {
    Serial.println("Temp is at a normal state"); 
  }

  else if (t>30) {
    Serial.println("Temp is higher than normal");
  }

  //writes the current date, time, temperature, hummidity to SD card
  myFile = SD.open("DATA.txt", FILE_WRITE); 
  lcd.setCursor(1, 0);
  lcd.print("Checking 1"); 
  myFile.print(now.day(), DEC);  
  myFile.print('/');
  myFile.print(now.month(), DEC);
  myFile.print('/');
  myFile.print(now.year(), DEC);
  myFile.print(',');
  myFile.print(' ');
  myFile.print(now.hour(), DEC);
  myFile.print(':');
  //inserts a 0 before minutes from 1-9 so that the time displays correctly, ie 12:05 instead of 12:5
  if (now.minute()<10) {
    myFile.print('0');
    myFile.print(now.minute(), DEC);
  }
  else {
    myFile.print(now.minute(), DEC);
  }
  myFile.print(',');
  myFile.print(' ');
  myFile.print(t);
  myFile.print(',');
  myFile.print(' ');
  myFile.print(h);
  myFile.println();
  myFile.close();

  //writes the current date, time, temperature, hummidity to the serial monitor 
  Serial.print(now.day(), DEC);
  Serial.print('/');
  Serial.print(now.month(), DEC);
  Serial.print('/');
  Serial.print(now.year(), DEC);
  Serial.print(',');
  Serial.print(' ');
  Serial.print(now.hour(), DEC);
  Serial.print(':');
  //inserts a 0 before minutes from 1-9 so that the time displays correctly, ie 12:05 instead of 12:5 
  if (now.minute()<10) {
    Serial.print('0');
    Serial.print(now.minute(), DEC);
  }
  else {
    Serial.print(now.minute(), DEC);
  }
  lcd.clear();
  lcd.setCursor(1, 0);

  Serial.print(',');
  Serial.print(' ');
  Serial.print("Temp: ");
  Serial.print(t);
  Serial.print(" *C");
  Serial.print(',');
  Serial.print(' ');
  Serial.print("Hum: ");
  Serial.print(h);
  Serial.println();
// temp and hum
  
  lcd.print("Temp  : "); 
  lcd.setCursor(9, 0);
  lcd.print(t);
  lcd.setCursor(11, 0);
  lcd.print(" *C");

  lcd.setCursor(1, 1);
  lcd.print("Hum   : "); 
  lcd.setCursor(9, 1);
  lcd.print(h);
  lcd.setCursor(11, 1);
  lcd.print(" Units");

// date
  lcd.setCursor(1, 2);
  lcd.print("D/M/Y : ");
  lcd.setCursor(9, 2);
  lcd.print((now.day(), DEC)); 
  lcd.setCursor(11, 2);
  lcd.print("/");
  lcd.setCursor(12, 2);
  lcd.print(now.month(), DEC); 
  lcd.setCursor(13, 2);
  lcd.print("/");
  lcd.setCursor(14, 2);
  lcd.print(now.year(), DEC); 
// time
  lcd.setCursor(1, 3);
  lcd.print("Time  : "); 
  lcd.setCursor(9, 3);
  lcd.print(now.hour(), DEC); 
  lcd.setCursor(10, 3);
  lcd.print(":");
  lcd.setCursor(11, 3);
  lcd.print(now.minute(), DEC); 

  delay(10000); //wait time until the device reads the temperature again
}