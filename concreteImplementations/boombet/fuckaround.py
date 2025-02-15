import requests

headers = {
    'accept': 'application/json',
    'accept-language': 'en-US,en;q=0.9',
    'authorization': 'Bearer',
    'origin': 'https://www.boombet.com.au',
    'priority': 'u=1, i',
    'referer': 'https://www.boombet.com.au/',
    'sec-ch-ua': '"Microsoft Edge";v="131", "Chromium";v="131", "Not_A Brand";v="24"',
    'sec-ch-ua-mobile': '?0',
    'sec-ch-ua-platform': '"Windows"',
    'sec-fetch-dest': 'empty',
    'sec-fetch-mode': 'cors',
    'sec-fetch-site': 'cross-site',
    'sp-deviceid': 'dev',
    'sp-id1': '0a8bd2cfc59c480889c9d76d307d53ad',
    'sp-platformid': '2',
    'user-agent': 'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/131.0.0.0 Safari/537.36 Edg/131.0.0.0',
}

params = {
    'checkHotBet': 'true',
    'includeForm': 'false',
}

response = requests.get('https://sb-saturn.azurefd.net/api/v3/race/event/36821397', params=params, headers=headers)

f = open("output.json", "w")
f.write(response.text)
f.close()