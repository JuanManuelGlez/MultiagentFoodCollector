from http.server import BaseHTTPRequestHandler, HTTPServer
import logging
import json
from json import JSONEncoder
import numpy as np

from Model.Model import foodColectionModel


class Server(BaseHTTPRequestHandler):

    def _set_response(self):
        self.send_response(200)
        self.send_header('Content-type', 'application/json')
        self.end_headers()

        WIDTH = 20
        HEIGHT = 20
        NUM_AGENTS = 5
        TOTAL_FOOD = 47
        ITER = 1500
        STEPS_TO_COMPLETE = 0
        STARTING_STEP = 5

        model = foodColectionModel(
            WIDTH, HEIGHT, NUM_AGENTS, TOTAL_FOOD, STARTING_STEP)

        for i in range(ITER):
            model.step()
            if STEPS_TO_COMPLETE != 0:
                break
            if model.depositQuantity == 47:
                STEPS_TO_COMPLETE = i

        all_data = model.datacollector.get_model_vars_dataframe()

        allData = all_data.get("AllData").to_list()

        response_data = {
            "data": allData
        }

        self.wfile.write(json.dumps(response_data).encode('utf-8'))

# ...

    def do_GET(self):
        self._set_response()
        # self.wfile.write("GET request for {}".format(
        #     self.path).encode('utf-8'))


def run(server_class=HTTPServer, handler_class=Server, port=8585):
    logging.basicConfig(level=logging.INFO)
    server_address = ('', port)
    httpd = server_class(server_address, handler_class)
    logging.info("Starting httpd...\n")
    try:
        httpd.serve_forever()
    except KeyboardInterrupt:
        pass
    httpd.server_close()
    logging.info("Stopping httpd...\n")


if __name__ == '__main__':
    from sys import argv

    if len(argv) == 2:
        run(port=int(argv[1]))
    else:
        run()
