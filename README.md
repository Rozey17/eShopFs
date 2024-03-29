# eShop

## Build and test the application

### Linux

```
$ chmod +x ./scripts/install.sh
$ ./scripts/install.sh
```

### Windows

Run the `build.bat` script in order to restore, build and test (if you've
selected to include tests) the application:

```
> .\scripts\install.bat
> .\scripts\build.bat
```

## Run the application

After a successful build you can start the web application by executing the following command in your terminal:

```
> .\scripts\run.bat
```

After the application has started visit [http://localhost:5000](http://localhost:5000) in your preferred browser.

## Watch the application

While development, run the script

```
> .\scripts\watch.bat
```