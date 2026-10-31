# Ronghe Yunxin HTTP interface reference v1.7.2

[中文原始文档](融合云信平台HTTP接口文档_V1.7.2.doc) | **English**

This is an unofficial translation of the supplier's v1.7.2 reference. Confirm current endpoints, authentication, callback requirements, and behavior with the provider.

## 1. Overview

Merchants using the HTTP interface must register and activate HTTP capabilities and SMS signatures on the Ronghe Yunxin platform before integration. Merchants joining through the China Unicom cloud marketplace receive HTTP capabilities automatically but must complete their access configuration.

The source specifies HTTP POST and UTF-8.

| Platform value | Meaning | Where to obtain it |
| --- | --- | --- |
| `cpcode` | Merchant code. | Merchant information page in the self-service platform. |
| `key` | Private key used to calculate the signature. | AccessKey on the self-service platform's system settings page. |

Keep the key secret. Example values below are placeholders.

## 2. Send a template SMS

The merchant calls the Ronghe Yunxin service:

- Source endpoint: `http://rcsapi.wo.cn:8000/umcinterface/sendtempletmsg`
- Method: POST
- Content-Type: `application/json`

The supplier document lists an HTTP endpoint. Confirm the current HTTPS endpoint and transport requirements with the provider before sending credentials or message data.

### Request

| Field | Type | Required | Description |
| --- | --- | --- | --- |
| `cpcode` | String | Yes | Merchant code assigned by the platform. |
| `msg` | String | Yes | Template parameter values, separated by ASCII commas when there are multiple values. |
| `mobiles` | String | Yes | Eleven-digit phone numbers separated by ASCII commas; at most 300. |
| `excode` | String | Yes | Channel extension code. It may be empty; send an empty string when unused. |
| `templetid` | String | Yes | Template SMS ID assigned by the platform. |
| `sign` | String | Yes | Lowercase MD5 digest of `cpcode + msg + mobiles + excode + templetid + key`, concatenated in that order without separators. |

The original describes MD5 as encryption; it is a digest and does not provide transport confidentiality.

```json
{
  "cpcode": "<CPCODE>",
  "msg": "183001,2,10010",
  "mobiles": "<PHONE_1>,<PHONE_2>",
  "excode": "00000",
  "templetid": "2",
  "sign": "<LOWERCASE_MD5>"
}
```

### Response

| Field | Type in source | Required | Description |
| --- | --- | --- | --- |
| `resultcode` | String | Yes | Error code; see the table below. |
| `resultmsg` | String | Yes | Error description. |
| `taskid` | String | Yes | Internal order ID used to match delivery reports. |

The source table calls `resultcode` a String, while its example uses numeric `0`. Confirm the actual response type with the provider.

```json
{"resultcode": 0, "resultmsg": "Success", "taskid": "<TASK_ID>"}
```

## 3. Delivery report callback

The merchant provides a callback endpoint and registers it in the self-service platform. Ronghe Yunxin calls it using POST.

- Content-Type: `text/plain`
- Payload: JSON array, at most 100 reports per batch.

| Field | Type | Required | Description |
| --- | --- | --- | --- |
| `state` | String | Yes | Delivery status: `0` means success; other values indicate failure. |
| `mobile` | String | Yes | Eleven-digit phone number. |
| `taskid` | String | Yes | Internal order ID used to match the send request. |
| `sortid` | String | Yes | Segment sequence number for a long message. |
| `time` | String | Yes | Report arrival time in `yyyyMMddHHmmss` format. |

Return JSON containing an integer `status`: `0` acknowledges receipt and `1` indicates an error.

```json
{"status": 0}
```

## 4. Inbound SMS callback

The merchant provides and registers a callback endpoint. Ronghe Yunxin calls it using POST.

- Content-Type: `text/plain`
- Payload: JSON array. The source states a maximum of 100 records per batch.

| Field | Type | Required | Description |
| --- | --- | --- | --- |
| `fisp` | String | Yes | Inbound port number. |
| `mobile` | String | Yes | Eleven-digit phone number. |
| `msg` | String | Yes | Inbound SMS content. |
| `time` | String | Yes | Inbound time in `yyyyMMddHHmmss` format. |

Return JSON with integer `status`: `0` for successful receipt or `1` for an error.

## 5. Error codes

| Code | Description |
| --- | --- |
| 0 | Success. |
| 301 | Invalid JSON format. |
| 302 | Invalid merchant information. |
| 304 | Missing or invalid `cpcode`. |
| 305 | Abnormal merchant status. |
| 306 | Merchant capability is unavailable or abnormal. |
| 307 | Insufficient template parameter values. |
| 308 | Missing `msg`. |
| 309 | Invalid `msg` length. |
| 310 | Content contains prohibited words. |
| 311 | Missing `mobiles`. |
| 312 | Invalid `mobiles` length. |
| 313 | Missing `excode`. |
| 314 | Missing `templetid`. |
| 315 | Missing `sign`. |
| 316 | Signature validation failed. |
| 317 | Application error. |

## 6. Java MD5 example

The supplier provides a Java example that encodes the concatenated signature input as UTF-8, computes an MD5 digest, and emits lowercase hexadecimal. This equivalent example uses placeholders instead of the source document's sample account values and key.

```java
import java.nio.charset.StandardCharsets;
import java.security.MessageDigest;
import java.security.NoSuchAlgorithmException;

public final class SignatureExample {
    public static String makeMd5(String plainText) throws NoSuchAlgorithmException {
        byte[] digest = MessageDigest.getInstance("MD5")
            .digest(plainText.getBytes(StandardCharsets.UTF_8));
        StringBuilder result = new StringBuilder();
        for (byte value : digest) {
            result.append(String.format("%02x", value & 0xff));
        }
        return result.toString();
    }

    public static void main(String[] args) throws NoSuchAlgorithmException {
        String cpcode = "<CPCODE>";
        String msg = "Template example,0001";
        String mobiles = "<PHONE_1>,<PHONE_2>";
        String excode = "";
        String templetid = "2";
        String key = System.getenv("RONGHE_API_KEY");
        String signatureInput = cpcode + msg + mobiles + excode + templetid + key;
        System.out.println(makeMd5(signatureInput));
    }
}
```
