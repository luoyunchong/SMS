# SMS57-SMGW client interface reference

[中文原始文档](SMS57-SMGW-客户接口说明文档.docx) | **English**

This is an unofficial English translation of the supplier document dated 2021-02-24. Confirm current endpoints, credentials, limits, and behavior with the provider.

## Common request details

The source specifies UTF-8 and an HTTP REST interface supporting GET and POST; it recommends POST. Request parameters must be URL-encoded using UTF-8.

- HTTP: `http://host:port/sms`
- HTTPS: `https://host/sms`; ask the provider whether HTTPS is supported.

Several operations use `account`, `password`, and `rt=json`. The document describes a plain credential mode and an MD5-derived mode. MD5 is a digest, not encryption; use a provider-supported secure transport and avoid including real credentials in examples or logs.

## 1. Send an SMS

Submit the same content to one or more recipients.

| Parameter | Meaning |
| --- | --- |
| `action` | Fixed value `send`. |
| `account` | Sending account supplied by the administrator. |
| `password` | Plain credential or `MD5(password + extno + content + mobile)`, according to the account mode. |
| `mobile` | Destination numbers separated by ASCII commas, up to 500 per submission. |
| `content` | SMS content, URL-encoded in UTF-8. |
| `extno` | Access number / SP service number, for example `106XXXXXX`. |
| `rt` | Response format, `json`. |

The response contains `status`, `balance`, and a `list` of items. Each item contains `mid` (message ID for matching status reports), `mobile`, and `result`. The balance is stated in li (厘). See the STATUS and RESULT tables below.

The source states a 500-number submission limit here; STATUS code 102 recommends keeping a submission within 200 numbers. Confirm the applicable limit with the provider.

## 2. Point-to-point send

Send individual content to each destination.

| Parameter | Meaning |
| --- | --- |
| `action` | Fixed value `p2p`. |
| `account` | Sending account. |
| `password` | Plain credential or `MD5(password + extno + mobileContentList)`. Remove line breaks from `mobileContentList` when calculating the MD5 value. |
| `mobileContentList` | Lines in the form `Mobile#Content`, separated by line breaks. |
| `extno` | Access number / SP service number. |
| `rt` | `json`. |

The response fields are the same as for normal SMS submission.

## 3. Query the account balance

| Parameter | Meaning |
| --- | --- |
| `action` | Fixed value `balance`. |
| `account` | Account. |
| `password` | Plain credential or `MD5(password)`. |
| `rt` | `json`. |

The response contains `status`, `balance` in li, and `chargeType`: `POSTCHARGE` means postpaid and `PRECHARGE` means prepaid.

## 4. Retrieve delivery reports

| Parameter | Meaning |
| --- | --- |
| `action` | Fixed value `report`. |
| `account` | Account. |
| `password` | Plain credential or `MD5(password)`. |
| `size` | Number of reports: default 1,000, minimum 10, maximum 10,000. |
| `rt` | `json`. |

The response contains `status`, `balance`, and a `list` that is empty when no reports are available.

| List field | Meaning |
| --- | --- |
| `flag` | `1` for a delivery report. |
| `mid` | Message ID matching the submission response. |
| `spid` | Owning account; may be ignored. |
| `mobile` | Phone number. |
| `accessCode` | Sending number. |
| `stat` | Delivery status; see STAT codes below. |
| `time` | Report time. |

The supplier's example request repeats `action=report`; verify the expected request format with the provider.

## 5. Retrieve inbound SMS

| Parameter | Meaning |
| --- | --- |
| `action` | Fixed value `mo`. |
| `account` | Account. |
| `password` | Plain credential or `MD5(password)`. |
| `size` | Number of inbound messages: default 1,000, minimum 10, maximum 1,000. The supplier table spells this field `szie`, while its example uses `size`; confirm the accepted name. |
| `rt` | `json`. |

The response contains `status`, `balance`, and a `list` that is empty when no inbound messages are available.

| List field | Meaning |
| --- | --- |
| `flag` | `0` for an inbound message. |
| `mid` | Inbound message ID; may be ignored. |
| `spid` | Owning account; may be ignored. |
| `mobile` | Sender number. |
| `accessCode` | Receiving SP service number. |
| `content` | Inbound message content. |
| `time` | Inbound message time. |

## 6. Receive inbound messages and delivery reports by push

Register a callback URL with the gateway. The gateway sends a POST containing an array of inbound-message and delivery-report objects.

| Field | Meaning |
| --- | --- |
| `flag` | `0` for an inbound message; `1` for a delivery report. |
| `mid` | For `flag=1`, the message ID matches the submission response; for `flag=0`, it identifies the inbound message. |
| `spid` | Owning account; may be ignored. |
| `mobile` | Phone number. |
| `accessCode` | Receiving SP service number. |
| `content` | Inbound content; not applicable when `flag=1`. |
| `stat` | Delivery status code; not applicable when `flag=0`. |
| `time` | Message time. |

After successful receipt, the client responds with plain text `OK`.

## 7. Retrieve statistics

| Parameter | Meaning |
| --- | --- |
| `action` | Fixed value `statis`. |
| `account` | Account. |
| `password` | Plain credential or `MD5(password)`. |
| `beginTime`, `endTime` | Date in the source's `yyyymmdd` format, for example `20210224`. The interval must not exceed 31 days. |
| `rt` | `json`. |

The response contains `status`, `balance`, and a `list` of statistics. The source spells the date field `statisTIme`.

| List field | Meaning |
| --- | --- |
| `statisTIme` | Date. |
| `total` | Total messages. |
| `success` | Successful messages. |
| `unknown` | Messages with unknown status. |
| `fail` | Failed messages. |
| `successRate` | Success rate. |

## STATUS codes

| Code | Meaning |
| --- | --- |
| 0 | Authentication succeeded. |
| 1 | Invalid message packet format. |
| 2 | IP authentication failed. |
| 3 | Incorrect account or credential. |
| 4 | Invalid version. |
| 5 | Other error. |
| 6 | Invalid access point, for example an account configured for CMPP access. |
| 7 | Account is disabled or otherwise abnormal. |
| 8 | Phone number is empty. |
| 9 | Content is empty. |
| 21 | Too many connections. |
| 30 | End time precedes start time or the statistics interval exceeds 31 days. |
| 31 | Statistics requests are too frequent; the source recommends once per hour. |
| 100 | Internal system error; contact the administrator. |
| 102 | Too many numbers in one submission; the source recommends no more than 200. |

## RESULT codes

| Code | Meaning |
| --- | --- |
| 0 | Submitted successfully. |
| 10 | Invalid originating/access number (`extno`). |
| 15 | Insufficient balance. |
| 17 | Invalid account signature. |

## STAT delivery codes

| Code | Meaning |
| --- | --- |
| `DELIVRD` | Message delivered. |
| `EXPIRED` | Message validity period expired. |
| `DELETED` | Message deleted. |
| `REJECTED` | Message rejected. |
| `MA:0001` | Number on the global blacklist. |
| `MA:0002` | Invalid content. |
| `MA:0003` | No downstream route found. |
| `MA:0004` | Unknown. |
| `MA:0005` | Invalid destination number format. |
| `MA:0006` | Rejected by the system. |
| `MA:0009` | Undefined error. |
| `MA:0011` | Unknown internal system error. |
| `MA:0012` | Anti-phishing check. |
| `MA:0013` | Invalid packet sequence. |
| `MA:0014` | Invalid OP_ISDN number range. |
| `MA:0021` | Invalid number format. |
| `MA:0022` | Number exceeded the half-hour sending limit. |
| `MA:0023` | Number on the customer blacklist. |
| `MA:0024` | Content was not registered with the provider. |
| `MA:0025` | SMS is not supported. |
| `MA:0026` | Segmented message assembly timed out. |
| `MA:0027` | Channel blacklist. |
| `MA:0028` | Number range on the global blacklist. |
| `MA:0029` | Number range on the channel blacklist. |
| `MA:0030` | Rejection report generated directly. |
| `MO:200` | Segmented SMS is not supported. |
| `MO:0254` | Forwarding submission timed out. |
| `MO:0255` | Connection dropped while forwarding the submission. |
| `MO:NNNN` | `NNNN` is an upstream gateway return value; ask the upstream provider for its meaning. |

## Common errors

- **10:** The access code `extno` is incorrect. Obtain the correct code from the gateway administrator.
- **17:** The signature is invalid. Include a signature registered by the gateway administrator, or have the administrator adjust account signature validation.
- **15:** The balance is insufficient. The gateway administrator can add credit or configure postpaid billing.
