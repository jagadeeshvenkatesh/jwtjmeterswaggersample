#
# Setting up the solution with Azure Active Directory for local testing
# - Creates an application in AAD for the Test API
# - Creates an application in AAD for the JMeter Test Client
# - Creates a service principal with a client secret for the JMeter Test Client
# - Grants the JMeter Test Client App access to the Test API in AAD
#
# Parameters:
# $1 = TenantId
# $2 = AAD Tenant Name (>>NAME<<.onmicrosoft.com - without the .onmicrosoft.com suffix)
# $3 = Test API App Audience URI
#

# Reading the parameters
tenantId=$1
tenantName=$2
jmeterClientSecret=$3

if [ -z $tenantId ] || [ -z $tenantName ] || [ -z $jmeterClientSecret ]; then
    echo 'Missing parameters'
    echo 'Correct usage: setup.sh aad-tenant-id aad-tenant-name jmeter-client-secret'
    exit 10
fi

# A few variables to enable easier change
testApiAudienceUri="api://marioszp-jmeter-test-backend-api"
testApiDisplayName="jmeter-test-api-backend"
jmeterClientAudienceUri="app://marioszp-jmeter-test-backend-tester"
jmeterTestClientDisplayName="jmeter-test-backend-tester"

#
# Main Shell Script Logic
#

echo '-----------------------------------'
echo '---- Creating Test Backend API ----'
echo '-----------------------------------'

echo 'Checking if app $testApiAudienceUri exists...'
existingTestApi=$(az ad app show --id $testApiAudienceUri --out json)
if [ -z $existingTestApi ]; then
    echo 'App does NOT exist, creating it ...'
    existingTestApi=$(az ad app create --identifier-uris $testApiAudienceUri \
                                       --display-name $testApiDisplayName \
                                       --available-to-other-tenants false \
                                       --out json)
else
    echo 'App does exist, just reading its APP ID'
fi 

# Get the AppId to configure the permissions after creating the test client for JMeter
testApiClientId=$(echo $existingTestApi | jq --raw-output ".appId")

# Now create a scope for the API based on a template manifest
scopeUuid=$(uuidgen)
# Update the UUID in the template manifest
# argJson="{ \"test\": \"mario\" }"
# echo $test | jq --argjson elem "$argJson" ".[0] | [ ., \$elem ]"
newPermission=$(cat ./backend.oauth2Permission.json \
                | sed -e "s/UUID/$scopeUuid/")
# Now add the permission by combining the existing permissions with the new one
existingApiPermissions=$(az ad app show --id $testApiClientId --query "oauth2Permissions" --out json)
# Create a file that contains the existing and the new permission combined by adding the new at the end

# Set the new oauth2Permissions in AAD
az ad app update --id $testApiClientId --set oauth2Permissions=@../trytesting/backend.oauth2Permissions.json

echo '----------------------------------------------------'
echo '---- Creating JMeter client registration in AAD ----'
echo '----------------------------------------------------'

echo 'Updating manifest based on template to allow the JMeter app to call the test API'
# TODO - update manifest_jmeter_client.json with the test API's client ID and copy it to the 'trytesting' directory 
cp ./manifest_jmeter_client.json ../trytesting
#sed 'do some magic with sed TODO'

echo 'Checking if app $jmeterClientAudienceUri exists...'
existingJmeterApp=$(az ad app list --identifier-uri $jmeterClientAudienceUri --out json)
if [ -z $existingJmeterApp ]; then
    echo 'App does NOT exist, creating it...'
    existingJmeterApp=$(az ad app create --identifier-uris $jmeterClientAudienceUri  \
                                         --display-name $jmeterTestClientDisplayName \
                                         --available-to-other-tenants false \ 
                                         --native-app \
                                         --password $jmeterClientSecret \
                                         --required-resource-access @manifest_jmeter_client.json \
                                         --out json)
else
    echo 'App does exist, just reading its app ID'
fi

# TODO - read the app ID with jq
jmeterClientId=$(jq dosomething)

# Now copy the JMeter JMX test file into the trytesting directory and update the settings with AAD data
cp ../jmeter.sh ../trytesting
#sed 'do some magic with sed TODO'