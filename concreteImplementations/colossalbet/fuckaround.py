from datetime import datetime, timedelta

import hmac
import hashlib
import time

import requests

def generate_auth(l):
    e = "colossalbet"
    r = "https://apicob.generationweb.com.au/GWBetService"
    n = "xnoh8dbr2pelqeuflv"

    key = f"{n}{l}"

    d = hmac.new(key.encode('utf-8'), r.encode('utf-8'), hashlib.sha1).hexdigest()

    f = f'clientKey={e}&timestamp={l}&signature={d}'
    return f

def get_data(rand, race_id):
    headers = {
        'accept': 'application/json, text/plain, */*',
        'accept-language': 'en-US,en;q=0.9',
        'authorization': generate_auth(rand),
        'origin': 'https://www.colossalbet.com.au',
        'priority': 'u=1, i',
        'referer': 'https://www.colossalbet.com.au/',
        'sec-ch-ua': '"Microsoft Edge";v="131", "Chromium";v="131", "Not_A Brand";v="24"',
        'sec-ch-ua-mobile': '?0',
        'sec-ch-ua-platform': '"Windows"',
        'sec-fetch-dest': 'empty',
        'sec-fetch-mode': 'cors',
        'sec-fetch-site': 'cross-site',
        'user-agent': 'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/131.0.0.0 Safari/537.36 Edg/131.0.0.0',
    }

    params = {
        'rand': rand,
    }

    response = requests.get(
        f'https://apicob.generationweb.com.au/GWBetService/r/b/GetEventRace/{race_id}/RunnerNum',
        params=params,
        headers=headers,
    )

    return response.text

race_id = str(25619328)
while True:
    time_stamp = str((datetime.now() + timedelta(milliseconds=1100)).timestamp())
    data = get_data(time_stamp, race_id)
    print(data)
    time.sleep(5)
    