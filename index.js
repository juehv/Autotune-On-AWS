const autotune = require('./_autotune_cloud');

exports.handler = async (event) => {
  
  console.log(JSON.stringify(event))
  
  // ns url
  const nsUrl = 'YOUR_NS_URL (without /api/v1)';

  // api secret
  const apiSecret = "YOUR_PASSWORT_HERE"; //"token=YOUR_TOKEN_HERE";//
  
  const noOfDays = 2; // don't do more than a few as the service gets killed when it's running for too long
  const upload = true;
  
  const result = await autotune(nsUrl, apiSecret, noOfDays, upload);
  
  console.log(JSON.stringify(result))
  
  const response = {
      statusCode: 200,
      body: "Finished",
  };
  
  return response;
};