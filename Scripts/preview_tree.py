#!/usr/bin/env python3
import sys

class Node:
	def __init__(self, name, children, level):
		self.name = name
		self.children = children
		self.level = level

	def print(self, level):
		print("   |"* level,end="")
		print(self.name)
		for c in self.children:
			c.print(level + 1)	

def main():
	if len(sys.argv) < 3:
		print("Usage: {} [csv.file] [starting node]".format(sys.argv[0]))
		return
	csvFilePath = sys.argv[1]
	starting_node = sys.argv[2]
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
