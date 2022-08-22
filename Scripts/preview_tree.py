#!/usr/bin/env python3
import sys

#https://jimblackler.net/treefun/index.html

class Node:
	separator = "   |"
	def __init__(self, name, children, level):
		self.name = name
		self.children = children
		self.level = level

	def print(self, level):
		print(Node.separator* level,end="")
		print(self.name)
		for c in self.children:
			c.print(level + 1)	

def main():
	ERR_MSG = "Usage: {} [csv.file] (starting node) (separator)".format(sys.argv[0])
	if len(sys.argv) < 2:
		print(ERR_MSG)
		return
	csvFilePath = sys.argv[1]
	if len(sys.argv) >= 3:
		starting_node = sys.argv[2]
	else:
		starting_node = "Root"
	if len(sys.argv) == 4:
		Node.separator = sys.argv[3]
	if len(sys.argv) > 4:
		print(ERR_MSG)
		return


	tree = {"Root":Node("Root",[], 0)}
	with open(csvFilePath, 'r') as f:
		for line in f:
			data = list(map(lambda s: s.strip('\n'), line.split(",")))
			if data[1] not in tree:
				tree[data[1]] = Node(data[1], [], tree[data[0]].level + 1)
			tree[data[0]].children.append(tree[data[1]])
	if starting_node not in tree:
		print("Starting node not in tree.")
		return
	tree[starting_node].print(1)
	return 0

main()
