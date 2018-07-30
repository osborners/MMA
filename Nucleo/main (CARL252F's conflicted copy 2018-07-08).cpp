#include "mbed.h"
#include "stepper.h"
#include "uart.h"

DigitalIn button(PC_13);
Stepper Bridge(PB_5, PB_3, PC_5, 800);
Stepper Track_r(PB_13, PA_10, PA_12, 800);
Stepper Track_l(PB_14, PA_11, PB_2, 800);
Stepper Hoist(PB_10, PB_4, PB_12, 800);
DigitalOut stepper_enable(PC_4);
PwmOut led(PA_5);
Uart uart(PA_2, PA_3);

void debounce() {
    while(button);
    wait_ms(200);
    while(!button);
    wait_ms(200);
}

void dual_home(void){
    while((Track_l.limit_switch&&Track_r.limit_switch)==0){
        Track_l.move_by(1,backwards,1);
        Track_r.move_by(1,backwards,1);
        if(Track_l.limit_switch == 1){
            Track_l.stop();
            Track_l.position = 0;
            Track_r.move_by(1,backwards,1);
        }
        else if(Track_r.limit_switch == 1){
            Track_r.stop();
            Track_r.position = 0;
            Track_l.move_by(1,backwards,1);
        }    
    }
    Track_l.stop();
    Track_r.stop();
    Track_l.position = 0;
    Track_r.position = 0;
    while((Track_l.limit_switch&&Track_r.limit_switch)==1){
        Track_l.move_by(1,forwards,1);
        Track_r.move_by(1,forwards,1);
        if(Track_l.limit_switch == 0){
            Track_l.stop();
            Track_l.position = 0;
            Track_r.move_by(1,forwards,1);
        }
        else if(Track_r.limit_switch == 0){
            Track_r.stop();
            Track_r.position = 0;
            Track_l.move_by(1,forwards,1);
        }    
    }
    Track_l.stop();
    Track_r.stop();
    Track_l.position = 0;
    Track_r.position = 0;
}

int main()
{
    
    int state = 1;
    while(1) {
        stepper_enable = 1;
        if(button == 0) {
            debounce();
            if(state == 3)
                state = 0;
            state ++;
        }
        else{
            if(state == 1) {
                Track_l.move_by_sync(1,forwards,30);
                led.period(2);
                led.pulsewidth(1);
                
            }
            if(state == 0){
                led.period(0);
                Track_l.stop();
            }
            if(state == 2){
                led.period(0);
                Track_l.stop();
            }
            if(state == 3){
                Track_l.move_by_sync(1,backwards,30);
                led.period(1);
                led.pulsewidth_ms(500);
            }
        }
    }
    return 0;
}