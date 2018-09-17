#include "mbed.h"

#ifndef STEPPER_H
#define STEPPER_H
#define forwards 0
#define backwards 1

class Stepper
{
 public:
    Stepper(PinName pd, PinName pp, PinName lmt, int f);
    void stop();
    void step();
    void run(int speed,int direction);
    void home(int Speed);
    void move_to(int location, int speed);
    void move_by(int amount, int dir, int speed);
    void move_to_sync(int location, int speed);
    void move_by_sync(int amount, int dir, int speed);
    void set_accel(float accel);
    int get_pos(void);
    int is_moving();
    void offset(int amount);
    volatile int position;
		void retract();
		void full_home(int direction);
 
 private:       
    DigitalOut step_pulse; 
		DigitalOut step_direction;
    volatile long movement;    
    float a;
    int o;
    float sp;
    volatile float cs;   
    float fac;
    Ticker t;
    Ticker a_t;
    void inc_s();
		void switch_triggered();
		InterruptIn limit_switch;

};

#endif
