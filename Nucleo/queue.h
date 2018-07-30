#ifndef _QUEUE_H
#define _QUEUE_H

const int QUEUE_SIZE = 64;

class Queue
{
public:
    Queue() : start(0), end(0) {}
    void push(char c) {
        if (! isFull()) {
            a[end] = c;
            end = (end + 1)%QUEUE_SIZE;
        }
    }

    char pop(void) {
        char c = '\0';
        if (! isEmpty()) {
            c = a[start];
            start = (start + 1)%QUEUE_SIZE;
        }
        return c;
    }
    bool isEmpty(void) {
        return end==start;
    }
    bool isFull(void) {
        return (end + 1)%QUEUE_SIZE == start;
    }
private:
    char a[QUEUE_SIZE];
    volatile int start;
    volatile int end;
};

#endif /* _QUEUE_H */
