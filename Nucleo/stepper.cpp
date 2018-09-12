# include "mbed.h"
# include "stepper.h"

DigitalOut LED_(PA_5);

Stepper::Stepper(PinName pd, PinName pp, PinName lmt, int f):
    step_direction(pd), step_pulse(pp), limit_switch(lmt)
{
    fac = f;
    position = 0;
    movement = 0;
		limit_switch.fall(callback(this, &Stepper::switch_triggered));
		limit_switch.rise(callback(this, &Stepper::retract));
		limit_switch.enable_irq();
}

void Stepper::inc_s()
{
    cs += a;
    if (cs >= sp) {
        a_t.detach();
        return;
    }
    t.attach_us(callback(this, &Stepper::step), 1000000 / fac / cs);
}

void Stepper::stop()
{
    a = 0;
    t.detach();
    a_t.detach();
		movement = 0;
}

void Stepper::step()
{
    if (movement == 0) {
        step_pulse = 0;
        stop();
    } else {
        step_pulse = ! step_pulse;
        if(movement > 0) movement = movement - 1;
        position += (step_direction > 0 ? -1 : 1);		
    }
}

void Stepper::move_to(int location, int speed)
{
    long l = (location + o)*fac;
    if (l > position) {
        move_by((l - position)/fac, forwards, speed);
    } else if (l < position) {
        move_by((position - l)/fac, backwards, speed);
    } else {
        return;
    }

}

void Stepper::move_by(int amount, int dir, int speed)
{
    step_direction = dir;
    movement = amount * fac;
    //position += amount * (dir ? 1 : -1);
    if (a == 0) {
        t.attach_us(callback(this, &Stepper::step), 1000000 / fac / speed);
    } else {
        sp = speed;
        cs = 0;
        a_t.attach_us(callback(this, &Stepper::inc_s), 10000);
    }
}


void Stepper::move_to_sync(int location, int speed)
{
    move_to(location, speed);
    while(is_moving());
}

void Stepper::move_by_sync(int amount, int dir, int speed)
{
    move_by(amount, dir, speed);
    while(is_moving());
}

void Stepper::home(int speed)
{
		Stepper::run(backwards, 30);

}

void Stepper::set_accel(int accel)
{
    a = accel;
}

int Stepper::is_moving()
{
    if (movement != 0) {
        return 1;
    } else {
        return 0;
    }
}

void Stepper::offset(int amount)
{
    o = amount;
    position = amount;
}

void Stepper::run(int speed,int direction)
{
    step_direction = direction;
    movement = -1;
    long d = 1000000 / fac / speed;
    t.attach_us(callback(this, &Stepper::step), d);
}

int Stepper::get_pos(void){
    return position/fac;
}

void Stepper::switch_triggered(){
		Stepper::stop();
		//LED_ = !LED_;
		Stepper::run(5, forwards);
}

void Stepper::retract(){
		Stepper::stop();
		position = 0;

}

void Stepper::full_home(int direction){
		Stepper::run(30, direction);
}
