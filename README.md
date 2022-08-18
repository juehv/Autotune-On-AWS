# Autotune for AWS Lambda

Inspired by [AutotuneWeb](https://github.com/MarkMpn/AutotuneWeb), this project ports autotune to AWS Lambda to run autotune automated in the cloud cheaply.
The autotune code is compiled from the [OpenAPS oref0](https://github.com/openaps/oref0) project v0.7.0 into a single js file with all dependencies.

## Getting Started
Steps to get the code running in AWS cloud.

### Basic NS Setup
The script will create / override the profile named "AutotuneWS Autosync".
It is recommended to copy your currenct profile and rename it before the first run.

### Profile conversion
You'll need a service to convert your profile from NS to a format autotune can work with. 
You'll find my modified files from AutotuneWeb in the subfolder [AutotuneWeb-Service](AutotuneWeb-Service).

For now, you may want to use my hosted service which is already used in the script.

If you want to use your own service, search for "https://jo1gisqwzc.execute-api.eu-central-1.amazonaws.com/Prod?nsUrl=" in the "_autotune_cloud" file and replace it with your service URL.

### Prepare Lambda package
Prepare a AWS lambda function for javascript. 
I used a cron service as trigger, running every night at 4:00, but you are free to choose whatever you want.

Put your nightscout url as variable nsUrl (line 8) and your api secret or token as variable apiSecret (line 11) in the "index.js" file.

Afterwards, you can upload the generated "_autotune_cloud.js" file, together with the index.js to the lambda function.

### Usage in AndroidAPS
With the automation feature you can load a profile (e.g. every morning).
As AnroidAPS is irgnoring updates of the profile you have to switch the profile to a different one and then switch back to the autotune profile.


## Contributing

This project is finished and will (most probably) not be updated any more.
Take a look at [OpenAPS](https://openaps.org/) if you feel like contributing to the open source looper community.
