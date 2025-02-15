import requests
import asyncio
import websockets
import json
import threading

def get_websocket_token(id):
    headers = {
        'accept': '*/*',
        'accept-language': 'en-US,en;q=0.9',
        'authorization': f'Bearer {id}',
        'cache-control': 'max-age=0',
        'origin': 'https://pointsbet.com.au',
        'priority': 'u=1, i',
        'referer': 'https://pointsbet.com.au/',
        'sec-ch-ua': '"Microsoft Edge";v="131", "Chromium";v="131", "Not_A Brand";v="24"',
        'sec-ch-ua-mobile': '?0',
        'sec-ch-ua-platform': '"Windows"',
        'sec-fetch-dest': 'empty',
        'sec-fetch-mode': 'cors',
        'sec-fetch-site': 'cross-site',
        'traceparent': '00-382cf67710894dd6849eb85ecd377f28-7ab2069b9b5048a1-01',
        'user-agent': 'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/131.0.0.0 Safari/537.36 Edg/131.0.0.0',
        'x-requested-with': 'XMLHttpRequest',
        'x-signalr-user-agent': 'Microsoft SignalR/8.0 (8.0.7; Unknown OS; Browser; Unknown Runtime Version)',
    }
    params = {
        'hub': 'signalrhub',
        'negotiateVersion': '1',
    }
    response = requests.post('https://push.au.pointsbet.com/client/negotiate', params=params, headers=headers)
    epic = json.loads(response.text)
    return epic

def get_access_token():
    headers = {
        'accept': '*/*',
        'accept-language': 'en-US,en;q=0.9',
        'content-type': 'text/plain;charset=UTF-8',
        'origin': 'https://pointsbet.com.au',
        'priority': 'u=1, i',
        'referer': 'https://pointsbet.com.au/',
        'sec-ch-ua': '"Microsoft Edge";v="131", "Chromium";v="131", "Not_A Brand";v="24"',
        'sec-ch-ua-mobile': '?0',
        'sec-ch-ua-platform': '"Windows"',
        'sec-fetch-dest': 'empty',
        'sec-fetch-mode': 'cors',
        'sec-fetch-site': 'cross-site',
        'user-agent': 'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/131.0.0.0 Safari/537.36 Edg/131.0.0.0',
    }
    data = '{}'
    response = requests.post('https://api.au.pointsbet.com/signalr/negotiate', headers=headers, data=data)
    return json.loads(response.text)['accessToken']

def set_race_watch(connection_id, race_id):
    headers = {
        'accept': '*/*',
        'accept-language': 'en-US,en;q=0.9',
        'content-type': 'application/json',
        'origin': 'https://pointsbet.com.au',
        'priority': 'u=1, i',
        'referer': 'https://pointsbet.com.au/',
        'sec-ch-ua': '"Microsoft Edge";v="131", "Chromium";v="131", "Not_A Brand";v="24"',
        'sec-ch-ua-mobile': '?0',
        'sec-ch-ua-platform': '"Windows"',
        'sec-fetch-dest': 'empty',
        'sec-fetch-mode': 'cors',
        'sec-fetch-site': 'cross-site',
        'user-agent': 'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/131.0.0.0 Safari/537.36 Edg/131.0.0.0',
    }

    json_data = {
        'GroupNames': [
            f'/racing/fixedoddsmarket/{race_id}',
            '/inPlaySportsList',
        ],
    }

    response = requests.post(
        f'https://api.au.pointsbet.com/signalr/{connection_id}/batch-subscribe',
        headers=headers,
        json=json_data,
    )
    return json.loads(response.text)

async def connect_websocket(token, access_token):
    uri = (
        f'wss://push.au.pointsbet.com/client/?hub=signalrhub&id={token["connectionToken"]}&access_token={access_token}'
    )
    headers = {
        'Upgrade': 'websocket',
        'Origin': 'https://pointsbet.com.au',
        'Cache-Control': 'no-cache',
        'Accept-Language': 'en-US,en;q=0.9',
        'Pragma': 'no-cache',
        'Connection': 'Upgrade',
        'User-Agent': 'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/131.0.0.0 Safari/537.36 Edg/131.0.0.0',
        'Sec-WebSocket-Version': '13',
        'Sec-WebSocket-Extensions': 'permessage-deflate; client_max_window_bits'
    }
    async with websockets.connect(uri, extra_headers=headers) as websocket:
        await websocket.send('{"protocol":"json","version":1}')
        set_race_watch(token['connectionId'], '77737040')
        print("Connected to the WebSocket server.")
        while True:
            await websocket.send('{"type":6}')
            response = await websocket.recv()
            print(f"Received: {response}")

def start_connection(connection, token):
    asyncio.get_event_loop().run_until_complete(connect_websocket(connection, token))

token = get_access_token()
access_token = get_websocket_token(token)

websocket_thread = threading.Thread(target=start_connection(access_token, token))
websocket_thread.start()



#f = open("output.json", "w")
#f.write(response.text)
#f.close()