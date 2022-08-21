#!/usr/bin/env python3
import sys

class Node:
	def __init__(self, name, children, level):
		self.name = name
		self.children = children
		self.level = level

	def print(self, level):
		print("\t"* level,end="")
		print(self.name)
		for c in self.children:
			c.print(level + 1)	

def main():
	if len(sys.argv) < 2:
		print("Usage: {} [csv.file]".format(sys.argv[0]))
		return
	csvFilePath = sys.argv[1]
	tree = {"Root":Node("Root",[], 0)}
	with open(csvFilePath, 'r') as f:
		for line in f:
			data = list(map(lambda s: s.strip('\n'), line.split(",")))
			if data[1] not in tree:
				tree[data[1]] = Node(data[1], [], tree[data[0]].level + 1)
			tree[data[0]].children.append(tree[data[1]])
	tree["Root"].print(1)
	return 0

main()
