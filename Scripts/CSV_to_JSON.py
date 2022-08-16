#!/usr/bin/env python

import pandas as pd
import sys

def main():
	if len(sys.argv) < 2:
		print("Usage: ./{} [csv.file] [json.file]".format(sys.argv[0]))
		return
	csvFilePath = sys.argv[1]
	jsonFilePath = sys.argv[2]

	df = pd.read_csv(csvFilePath, header=0)
	df.to_json(jsonFilePath, orient="records", lines=True)
	return

if __name__=="__main__":
	main()
