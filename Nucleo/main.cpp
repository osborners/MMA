#define HomeSpeed 30
#define MoveSpeed 50
#include "mbed.h"
#include "stepper.h"


DigitalIn button(PC_13);
Stepper Track_l(PB_5, PB_3, PC_5, 800);
Stepper Track_r(PB_13, PA_10, PA_12, 800);
Stepper Bridge(PB_14, PA_11, PB_2, 800);
Stepper Hoist(PB_10, PB_4, PB_12, 800);
DigitalOut stepper_enable(PC_4);
Serial uart(PA_2 , PA_3);
DigitalOut LED(PA_5);
//Serial pc(USBTX, USBRX);
Ticker Don;

void debounce() {
    while(button);
    wait_ms(200);
    while(!button);
    wait_ms(200);
}

void dual_home(){
    while((Track_l.limit_switch&&Track_r.limit_switch)==0){
        Track_l.run(30, backwards);
        Track_r.run(30, backwards);
        if(Track_l.limit_switch == 1){
            Track_l.stop();
            Track_l.position = 0;
        }
        else if(Track_r.limit_switch == 1){
            Track_r.stop();
            Track_r.position = 0;
        }
    }
    while((Track_l.limit_switch&&Track_r.limit_switch)==1){
        Track_l.run(30, forwards);
        Track_r.run(30, forwards);
        if(Track_l.limit_switch == 0){
            Track_l.stop();
            Track_l.position = 0;
        }
        else if(Track_r.limit_switch == 0){
            Track_r.stop();
            Track_r.position = 0;
        }
    }
}

void flushSerialBuffer(void){ 
    char char1 = 0; 
    while (uart.readable()){ 
        char1 = uart.getc(); 
        } return; 
}

char posFlag = 0;
void update_pos(void){
		posFlag = 1;
}

void display_pos(void){
    if(Bridge.is_moving() || Hoist.is_moving() || Track_l.is_moving()){
        uart.printf("x%i\n", Bridge.get_pos());
        uart.printf("y%i\n", Track_l.get_pos());
        uart.printf("z%i\n", Hoist.get_pos());
    }
		posFlag = 0;
}

void jog_command(char command){
    if(command == 'L')
        Bridge.run(50,backwards);
    else if(command == 'R')
        Bridge.run(50,forwards);
    else if(command == 'U'){
        Track_l.run(30,backwards);
        Track_r.run(30,backwards);
    }
    else if(command == 'D'){
        Track_l.run(30,forwards);
        Track_r.run(30,forwards);
    }
    else if(command == 'C')
        Hoist.run(30,forwards);
    else if(command == 'X')
        Hoist.run(30,backwards);
    else if(command == 'N')
        Bridge.position = 0;
    else if(command == 'A'){
        Track_l.position = 0;
        Track_r.position = 0;
    }
    else if(command == 'P')
        Hoist.position = 0;
    else if(command == 'S'){
        Bridge.stop();
        Track_l.stop();
        Track_r.stop();
        Hoist.stop();
        LED = !LED;
    }
}

void readLine(char* l) {
	char i = 0;
	while(1) {
		while(uart.readable()) {
			l[i] = uart.getc();
			if(l[i] == '\n') return;
			i++;
		}
	}
}

int main(){
    Don.attach(&update_pos, 0.5);
    uart.baud(9600);
    stepper_enable = 1;
	
	  char value[14];
	
    while(1){
        uart.printf("Waiting for Command\n");
        char value[14];
        int bridge_cooard;
        int track_cooard;
        int hoist_cooard;
			
        readLine(value);
			
        bridge_cooard = (value[3]-48)*100+(value[4]-48)*10+(value[5]-48);
        track_cooard = (value[7]-48)*100+(value[8]-48)*10+(value[9]-48);
        hoist_cooard = (value[11]-48)*100+(value[12]-48)*10+(value[12]-48);

			if(value[0]=='J'& value[1]=='G'){
            uart.printf("Jog Mode\n");
            char temp;
						while(!uart.readable()) if (posFlag) display_pos();
            temp = uart.getc();
            while(temp != 'Z'){
                jog_command(temp);
								while(!uart.readable()) if (posFlag) display_pos();
                temp = uart.getc();
            }
            uart.printf("DonAld\n");
            }
        else if(value[0]=='M'& value[1]=='T'){
            Bridge.move_to(bridge_cooard,MoveSpeed);
            Track_l.move_to(track_cooard,MoveSpeed);
            Track_r.move_to(track_cooard,MoveSpeed);
            Hoist.move_to(hoist_cooard,MoveSpeed);
        }
        else if(value[0]=='M'& value[1]=='B'){
            int bridge_dir;
            int track_dir;
            int hoist_dir;
            if(value[2] == '-'){
                bridge_dir = backwards;
            }
            else{
                bridge_dir = forwards;
            }
            if(value[6] == '-'){
                track_dir = backwards;
            }
            else{
                track_dir = forwards;
            }
            if(value[10] == '-'){
                hoist_dir = backwards;
            }
            else{
                hoist_dir = forwards;
            }
            Bridge.move_by(bridge_cooard,bridge_dir,MoveSpeed);
            Track_l.move_by(track_cooard,track_dir,MoveSpeed);
            Track_r.move_by(track_cooard,track_dir,MoveSpeed);
            Hoist.move_by(hoist_cooard,hoist_dir,MoveSpeed);
						while(Bridge.is_moving() || Hoist.is_moving() || Track_l.is_moving() || Track_r.is_moving());
						wait_ms(100);
						uart.printf("a\n");
        }

    }    
    return 0;
}
