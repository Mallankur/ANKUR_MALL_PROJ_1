// seed db collection and data. Only needed for testing or if we want some default business accounts
db = new Mongo().getDB("businessaccount");
db.createCollection('ssoconfigurations', { capped: false });
db.ssoconfigurations.createIndex({ name: 1 }, { unique: true })
db.ssoconfigurations.insert([
	{
		"_id": "4954749c-af85-4aa5-b3fb-27dbd6356471",
		"name": "intensa",
		"type": "saml2",
		"domains": ["*.intensa.com"],
		"saml2": {
			"entityID": "http://localhost:8080/realms/TestSAML",
			"metadataLocation": "http://localhost:8080/realms/TestSAML/protocol/saml/descriptor"
		}
	},
	{
		"_id": "0f026b91-7014-4dc5-afff-bd5f4a4f014d",
		"name": "adform",
		"type": "oidc",
		"domains": ["*.adform.com"],
		"oidc": {
			"authority": "id.adform.com",
			"clientId": "passport",
			"redirect_uri": "",
			"scope": ["oidc", "profile"]
		}
	}
]);

db.createCollection('businessaccounts', { capped: false });
db.businessaccounts.createIndex({ type: 1, name: 1 }, { unique: true })
db.businessaccounts.insert([
	{
		"legacyId": NumberInt(100),
		"type": "Agency",
		"name": "Tenant",
		"active": true,
		"ssoConfigurationId": "0f026b91-7014-4dc5-afff-bd5f4a4f014d"
	},
	{
		"legacyId": NumberInt(100),
		"type": "Publisher",
		"name": "Tenant",
		"active": true,
		"ssoConfigurationId": "0f026b91-7014-4dc5-afff-bd5f4a4f014d"
	},
	{
		"legacyId": NumberInt(100),
		"type": "Agency",
		"name": "IntensaTenant",
		"active": true,
		"ssoConfigurationId": "4954749c-af85-4aa5-b3fb-27dbd6356471"
	},
	{
		"legacyId": NumberInt(100),
		"type": "Publisher",
		"name": "IntensaTenant",
		"active": true,
		"ssoConfigurationId": "4954749c-af85-4aa5-b3fb-27dbd6356471"
	}
]);