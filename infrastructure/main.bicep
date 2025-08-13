param location string = 'australia east'

param appServiceAppName string = 'school-demo-${uniqueString(resourceGroup().id)}'

var appServicePlanName string = 'school-demo-plan'



resource appServicePlan 'Microsoft.Web/serverfarms@2024-11-01' = {
  name: appServicePlanName
  location: location
  sku:{
    name: 'F1'
  }
}

resource appServiceApp 'Microsoft.Web/sites@2024-11-01' = {
  name: appServiceAppName
  location: location
  properties: {
    serverFarmId: appServicePlan.id
    httpsOnly: true
  }
}

output appServiceAppName string = appServiceAppName
output appServiceAppHostName string = appServiceApp.properties.defaultHostName
