# Ray-tracing-in-one-weekend-CSharp

A C# port of Peter Shirley's [Ray Tracing in One
Weekend](https://www.amazon.com/gp/product/B01B5AODD8/ref=as_li_tl?ie=UTF8&camp=1789&creative=9325&creativeASIN=B01B5AODD8&linkCode=as2&tag=inonwe09-20&linkId=OPNJXXJY2IBCMEGE) ([book](https://drive.google.com/drive/folders/14yayBb9XiL16lmuhbYhhvea8mKUUK77W)
and [C++ code](https://github.com/RayTracing/raytracinginoneweekend) available for free).

Running the application generates a random scene:

![Random scene](Random-scene.png)

If you feel a bit rusty on the math, you may want to start with a few posts I
wrote while working through the book:

- [Ray tracing: Computing ray-circle intersections](http://bugfree.dk/blog/2018/09/01/ray-tracing-computing-ray-circle-intersections)
- [Ray tracing: Computing ray-sphere intersections](http://bugfree.dk/blog/2018/09/03/ray-tracing-computing-ray-sphere-intersections)

## Getting started

    $ git clone https://github.com/ronnieholm/Ray-tracing-in-one-weekend-CSharp.git
    $ cd Ray-tracing-in-one-weekend-CSharp.git
    $ dotnet run -c release > out.ppm
    $ display out.ppm

On my Intel Core i7-7560U CPU @ 2.40GHz laptop running Linux and .NET Core 2.2,
raytracing takes about four minutes.

## Benchmarking

For benchmarking, writing to standard output should be disabled in
``Program.cs``:

    % time dotnet run -c release > /dev/null

    real    3m57.752s
    user    3m57.106s
    sys     0m0.461s

Peter Shirley's C++ version:

    % git clone https://github.com/RayTracing/raytracinginoneweekend.git
    % cd raytracinginoneweekend/src/

Comment out the last ``std::cout`` statement in ``main.cc``.

    % g++ -O2 main.cc 
    % time ./a.out > /dev/null 

    real    1m13.045s
    user    1m12.789s
    sys     0m0.056s

In terms of raw computation power, .NET Core doesn't come close to C++. At least
not while staying true to the C++ code structure.