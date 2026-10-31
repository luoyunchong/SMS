# Cloud MAS HTTPS SMS interface

[中文原始文档](MAS_HTTPS短信接口文档.docx) | **English**

This is an unofficial translation of the supplier document bundled in this repository. Field names, response codes, and limits are reproduced as written; confirm current behavior with the provider.

## 1. Send a normal SMS

An HTTPS client submits an SMS task to Cloud MAS. The platform validates the request and, if validation succeeds, forwards it to the SMS gateway.

- Endpoint: `https://****:****/sms/submit`
- Method: POST
- Data format: JSON, described by the source as Base64-encoded.

### Request: one-to-one or many-to-one

| Field | Type | Description |
| --- | --- | --- |
| `ecName` | String | Enterprise name. |
| `apId` | String | API account username. |
| `mobiles` | String | Recipient phone numbers separated by ASCII commas, up to 5,000 per batch. |
| `content` | String | SMS content. |
| `sign` | String | Signature code, obtained in Cloud MAS under Management → Interface Management → SMS Access User Management. |
| `addSerial` | String | Extension code. For exact matching use an empty string. For fuzzy matching use an empty string or a custom extension. The service code and extension together must not exceed 20 digits. |
| `mac` | String | Validation value: concatenate `ecName`, `apId`, `secretKey`, `mobiles`, `content`, `sign`, and `addSerial` in that order without separators, then compute lowercase 32-character MD5. |

### Request: many-to-many

The request uses the same fields and MAC order. Set `mobiles` to an empty string and put a mapping of phone numbers to messages in `content`. The source specifies fewer than 1,000 numbers in this mode.

```json
{
  "<PHONE_1>": "Hello, China!",
  "<PHONE_2>": "Hello, world!",
  "<PHONE_3>": "Hello, Cloud MAS!"
}
```

The mapping above illustrates the content value; the source represents it as a JSON string.

### Response

| Field | Type | Description |
| --- | --- | --- |
| `rspcod` | String | Response status; see the codes below. |
| `msgGroup` | String | Cloud MAS batch identifier used to associate the request and delivery reports; empty if validation fails. |
| `success` | boolean | Data-validation result. |

| `rspcod` | Meaning |
| --- | --- |
| `IllegalMac` | MAC validation failed. |
| `IllegalSignId` | Invalid signature code. |
| `InvalidMessage` | Invalid message or request parsing failed. |
| `InvalidUsrOrPwd` | Invalid username or credential. |
| `NoSignId` | No matching signature information. |
| `success` | Validation succeeded. |
| `TooManyMobiles` | More than 5,000 phone numbers; use at most 5,000. |

## 2. Send a template SMS

- Endpoint: `https://****:****/sms/tmpsubmit`
- Method: POST
- Data format: JSON, described by the source as Base64-encoded.

| Field | Type | Description |
| --- | --- | --- |
| `ecName` | String | Enterprise name. |
| `apId` | String | API account username. |
| `templateId` | String | Template ID. Create a template under SMS → Template SMS → Template Management, submit it for review, and use the ID received after approval. |
| `mobiles` | String | Recipient phone numbers separated by ASCII commas, up to 5,000 per batch. |
| `params` | String | Template variables, for example `["param1","param2"]`. For a template without variables use `[""]`. |
| `sign` | String | Signature code obtained from SMS Access User Management. |
| `addSerial` | String | Extension code, subject to the same matching rules and 20-digit combined limit described above. |
| `mac` | String | Concatenate `ecName`, `apId`, `secretKey`, `templateId`, `mobiles`, `params`, `sign`, and `addSerial` in that order without separators; compute lowercase 32-character MD5. |

The response contains `rspcod`, `success`, and a batch identifier described the same way as above. The source spells the batch field `mgsGroup` in this section, but `msgGroup` in the normal-SMS section; confirm the actual field with the provider. The response-code table is the same as above.

## 3. Retrieve delivery reports

The client initiates the request; Cloud MAS returns a collection of 0–1,000 reports.

- Endpoint: `https://****:****/sms/report`
- Method: POST
- Data format: JSON, described by the source as Base64-encoded.

| Request field | Type | Description |
| --- | --- | --- |
| `ecName` | String | Enterprise name. |
| `apId` | String | API account username. |
| `secretKey` | String | API account credential. |

| Response field | Type | Description |
| --- | --- | --- |
| `reportStatus` | String | Successful delivery status: `DELIVRD`. |
| `mobile` | String | Recipient number in the report; one number per item. |
| `submitDate` | String | Submission time, `yyyyMMddHHmmss`. |
| `receiveDate` | String | Receipt time in the same format. |
| `errorCode` | String | Failure status code, for example `DB:0140`. |
| `msgGroup` | String | Unique batch identifier. The supplier's description is incomplete. |

## 4. Retrieve inbound SMS

The client initiates the request; Cloud MAS returns a collection of 0–1,000 inbound messages.

- Endpoint: `https://****:****/sms/deliver`
- Method: POST
- Data format: JSON, described by the source as Base64-encoded.
- Request fields: `ecName`, `apId`, and `secretKey`, as above.

| Response field | Type | Description |
| --- | --- | --- |
| `smsContent` | String | Inbound message content. |
| `mobile` | String | Sender phone number; one number per item. |
| `sendTime` | String | Send time, `yyyy-MM-dd HH:mm:ss`. |
| `addSerial` | String | Inbound service code. |

## Supplier notes

- One-to-one sends one message to one number. Many-to-one sends the same message to a batch; the source says fewer than 5,000 numbers here, while its field table says at most 5,000. Confirm the limit with the provider.
- Many-to-many associates each number with its own content and supports fewer than 1,000 numbers.
- The source says requests for delivery reports and inbound messages may be made only once within five seconds.
- The supplier document instructs clients to ignore certificate verification and says the provider does not supply a certificate/key. Disabling verification permits server impersonation; confirm a secure trust configuration with the provider. See [RFC 9110 section 4.3.4](https://www.rfc-editor.org/rfc/rfc9110.html#name-https-certificate-verification).
- Base64 is an encoding, not encryption.
