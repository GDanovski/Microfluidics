//variable for the incomming command
String currentLine = "";
String protocol = "";
//define pins for the valves
const int valve1=26;
const int valve2=25;
const int valve3=33;
const int valve4=32;
//define the pin for the drain
const int drain = 27;
//difine the start pin
const int startProtocol = 34;
//define LED indicators
const int protocolRunning = 16;
const int protocolLoaded = 17;

void setup() {
  Serial.begin(115200); // opens serial port, sets data rate to 115200 bps
  pinMode (valve1, OUTPUT);
  pinMode (valve2, OUTPUT);
  pinMode (valve3, OUTPUT);
  pinMode (valve4, OUTPUT);
  pinMode (drain, OUTPUT);
  pinMode (protocolRunning, OUTPUT);
  pinMode (protocolLoaded, OUTPUT);
  
  pinMode (startProtocol, INPUT);
  
  resetPins();
}
void loop() {
        if(digitalRead(startProtocol) == HIGH){
          decodeProtocol();
        }
        
        if (Serial.available() > 0) 
        {
          char c = Serial.read();
          if (c == ';')
          {                   
            if(currentLine != "")
            {
              if(currentLine[0] == 'p')
              {
                protocol = currentLine.substring(1);
                Serial.println("Protocol loaded!");                
                digitalWrite (protocolLoaded, HIGH); // turn off the pin
              }
              else
              {
                decodeString(currentLine);
              }                
              currentLine = "";
            }                       
            delay(10);                                   
          }        
          else
          {
            currentLine += c;
          }
      }    
}
void resetPins(){  
  digitalWrite (drain, LOW); // turn off the pin
  digitalWrite (valve1, LOW); // turn off the pin
  digitalWrite (valve2, LOW); // turn off the pin
  digitalWrite (valve3, LOW); // turn off the pin
  digitalWrite (valve4, LOW); // turn off the pin

  digitalWrite (protocolRunning, LOW); // turn off the pin
  digitalWrite (protocolLoaded, LOW); // turn off the pin
}
void decodeString(String cmd){
  
char object = cmd[0];
char state = cmd[1];
int value = 0;
if(cmd.length()>2)
  value = (cmd.substring(2)+"000").toInt();;

switch(object){
  case 'v':    
    if(state == 't'){
      if(value == 1000)
         digitalWrite (valve1, HIGH);
      else if(value == 2000)
         digitalWrite (valve2, HIGH);
      else if(value == 3000)
         digitalWrite (valve3, HIGH);
      else if(value == 4000)
         digitalWrite (valve4, HIGH);
    }
    else if(state == 'f'){
      if(value == 1000)
         digitalWrite (valve1, LOW);
      else if(value == 2000)
         digitalWrite (valve2, LOW);
      else if(value == 3000)
         digitalWrite (valve3, LOW);
      else if(value == 4000)
         digitalWrite (valve4, LOW);
    }
    delay(15);
    break;
  case 'd':
    if(state == 't'){
       digitalWrite (drain, HIGH);
    }
    else if(state == 'f'){
       digitalWrite (drain, LOW);
    }    
    delay(15);
    break;
  case 'w':
    delay(value);
    break;
  case 's':
    if(state == 't'){
       decodeProtocol();
    }
    else if(state == 'f'){
       protocol = "";
    }
    break;
  default:
    break;
  }  
}
void decodeProtocol(){
  String myProtocol = protocol + "|";
  String cmd = "";
  
  Serial.println("Protocol started!");
  digitalWrite (protocolRunning, HIGH); // turn off the pin
  
  for(int i = 0; i< myProtocol.length(); i++){
    if(myProtocol[i]=='|'){      
      Serial.println(" - " + cmd);
      decodeString(cmd);
      cmd = "";
    }
    else
    {
      cmd +=  myProtocol[i];
    }
  }

  protocol="";
  digitalWrite (protocolLoaded, LOW); // turn off the pin
  digitalWrite (protocolRunning, LOW); // turn off the pin
  Serial.println("Protocol finished!");
}
