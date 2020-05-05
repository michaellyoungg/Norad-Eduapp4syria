#!/usr/bin/python
import os
import codecs
import xml.etree.ElementTree as ET
import xml.dom.minidom as minidom
import random
from sys import argv
import csv
oldTargets = []

def markAsUsed (target):
    oldTargets.append(target)

def isAlreadyUsed (target):
    if target not in oldTargets:
        return False
    else:
        return True

def clearOldTargets ():
    oldTargets[:]=[]

def chooseTarget (lst, isSoundTrial):
        if isSoundTrial:
            return chooseOldTarget(lst)
        else:
            return chooseNewTarget(lst)

def chooseNewTarget (lst):
    #parsedList = [x for x in lst if x not in oldTargets]
    test = lst[random.randrange(len(lst))]
    markAsUsed(test)
    return test

def chooseOldTarget (lst):
    parsedList = [x for x in lst if x in oldTargets]
    test = lst[random.randrange(len(lst))]
    return test


def checkGroupMax (groupNum):
    if groupNum >= len(groups):
        return len(groups)
    else:
        return groupNum

def letterFill (group, root, isSoundTrial):
    segments = root[0]
    group = checkGroupMax(group)
    count = 0
    for subTree in segments:
        count = count +1
        #print "puzzle: " + str(segments.getchildren().index(subTree)) #DEBUG
        #print "group: " + str(group) #DEBUG
        letters = groups[group-1]['letters']
        target = chooseTarget(letters, isSoundTrial)
        target = target.lower()
        foilCount = countStones(subTree.find("Stones"), target)

        foils = addFoils(foilCount, letters, target, "letter")
        #print "foil count: " + str(len(foils)) #DEBUG
        setPuzzle(target, [],foils,subTree, count)
    #print target

def letterWordFill (group, root,):
    segments = root[0]
    group = checkGroupMax(group)
    puzzleCount = 0
    for subTree in segments:
        puzzleCount = puzzleCount +1
        #print "puzzle: " + str(segments.getchildren().index(subTree)) #DEBUG
        letters = groups[group-1]['letters']
        if len(groups[group-1]['syllables']) >1:
            words = groups[group-1]['words'] + groups[group-1]['syllables']
        else:
            words= groups[group-1]['words']
        targetWord = findWordWithLetter(words, letters, False)
        targetLetter = targetWord[0]
        if (targetWord == "NONE"):
            print("WARNING: COULD NOT FIND WORD FOR PUZZLE " + str(puzzleCount) + " WITH TARGET " + targetLetter)
        targetLetter = targetLetter.lower()
        targetWord = targetWord.lower()
        targetWord= targetWord.replace(targetLetter, "", 1)
        targetArray = list(targetWord)
        targetArray = ['X'] + targetArray
        foilCount = countStones(subTree.find("Stones"), targetLetter)
        foils = addFoils(foilCount, letters, targetLetter, "letter")
        setPuzzle(targetLetter, targetArray, foils, subTree, puzzleCount)


def wordFill (group, root, isSoundTrial):
    segments = root[0]
    group = checkGroupMax(group)
    count = 0
    for subTree in segments:
        count = count+1
        #print "puzzle: " + str(segments.getchildren().index(subTree)) #DEBUG
        lettersList = groups[group-1]['letters']
        if len(groups[group-1]['syllables']) >1:
            wordsList = groups[group-1]['words'] + groups[group-1]['syllables']
        else:
            wordsList = groups[group-1]['words']
        targetSyllable = chooseTarget(wordsList, isSoundTrial)
        targetSyllable = targetSyllable.lower()
        requiredLetters = list(targetSyllable)
        foilCount = countStones (subTree.find('Stones'), requiredLetters)
        foils = addFoils(foilCount, letters, requiredLetters, "Word")
        setPuzzle(requiredLetters, [], foils, subTree, count)


def countStones (subTree, requiredLetters):
    stoneList = subTree.findall('string')
    count = 0
    for s in stoneList:
        if not isGameplayText(s.text):
            count = count + 1
    if(type(requiredLetters) == list):
        count = count - len(requiredLetters)
    else:
        count = count -1
    return count


def findWordWithLetter(list, letters, isSoundTrial):
    letterCounter = 0
    set = []
    while (letterCounter < len(letters)):
        targetLetter = chooseTarget(letters, isSoundTrial)
        if len(targetLetter) == 1:
            for w in list:
                if w.find(targetLetter) == 0:
                    set.append(w)
                    break;
        elif len(targetLetter) == 2:
            for w in list:
                if w.find(targetLetter[0]) == 0:
                    set.append(w)
                    break;
        letterCounter = letterCounter + 1
    if(len(set) == 0):
        return "NONE"
    return (set[random.randrange(len(set))])


def removeSpaces (lst):
    lst = [x.lstrip()for x in lst if x != ' ']
    lst = [x.strip() for x in lst if x != ' ']
    lst = [x.lower() for x in lst if x != ' ']
    return lst

def isGameplayText (string):
    if(string != 'BonusLetter'
    and string != 'FireWrongLetter'
    and string != 'MagnetLetter'
    and string != 'Shield'):
        return False
    else:
        return True

def setPuzzle (requiredLetters, allLetters, stones, subTree, puzzleNum):
    req = "MonsterRequiredLetters"
    allLet = "MonsterAllLetters"
    setSubElements( subTree.find(req), requiredLetters, req, puzzleNum)
    if(len(allLetters) != 0):
        setSubElements(subTree.find(allLet), allLetters, allLet, puzzleNum)
    setSubElements(subTree.find("Stones"), stones, "Stones", puzzleNum)

def setSubElements (subTree, csv, sectionName, puzzleNum):
    #print "setting: " + sectionName #DEBUG
    lettersAdded = 0
    for child in subTree.findall('string'):
        spawnList = [];
        index = subTree.getchildren().index(child)
        if (sectionName == "Stones"):
            spawnList.append(str(child.get('spawnId')))
        if(not isGameplayText(child.text)):
            if ((type(csv) == str and index > 0) or index  >= len(csv)):
                subTree.remove(child)
            elif type(csv)== str:
                child.text = csv #.decode('utf-8')
                lettersAdded = lettersAdded + 1
            elif(csv[index] !=""):
                child.text = csv[index] #.decode('utf-8')
                lettersAdded = lettersAdded + 1
            else:
                subTree.remove(child)
        else:
            if((type(csv) == str and index <= 0) or index  < len(csv)):
                e = ET.SubElement(subTree, 'string')
                if(sectionName == "Stones"):
                    print ("stone added to Puzzle" + str(puzzleNum) +"") #DEBUG
                if (type(csv) == str):
                    e.text = csv
                elif (csv[index] != ""):
                    e.text = csv[index]
                spawnId = setSpawnId(spawnList)
                spawnList.append(spawnId)
                e.set('spawnId', spawnId)
            lettersAdded = lettersAdded + 1
    if ((type(csv) == str and len(subTree.getchildren()) > 1) or (type(csv) ==
       list and len(subTree.getchildren()) <= len(csv))):
       setLeftovers(subTree, csv, sectionName, lettersAdded, spawnList, puzzleNum)

def setSpawnId (spawnList):
    isUnique = False
    spawn = 0
    while (not isUnique):
        spawn = random.randrange(20,32, 2)
        try:
            spawnList.index(spawn-4)
        except ValueError:
            isUnique = True
    return str(spawn)

def setLeftovers (subTree, csv, sectionName, lettersAdded, spawnList, puzzleNum):
    #print "adding strings!" #DEBUG
    if(len(subTree.getchildren()) == 0):
        leftovers = csv
    else:
        #print('pre leftovers: ' + str(csv)) #DEBUG
        if type(csv) == list:
            leftovers = csv[lettersAdded: ]
            #print('post leftovers: ' + str(leftovers)) #DEBUG
        else:
            leftovers = csv
            #print "no change" #DEBUG
    for l in leftovers:
        if (l != ""):
            e = ET.SubElement(subTree, 'string')
            e.text = l #.decode('utf-8')
            if(sectionName == 'Stones'):
                print ("stone added to Puzzle" + str(puzzleNum) + "") #DEBUG
                spawnId = setSpawnId(spawnList)
                e.set('spawnId', spawnId)
                spawnList.append(spawnId)
def isTarget (test, targetList):
    for t in targetList:
        if (t == test):
            return True
    return False

def addFoils(numFoils, letterGroup, targetList, gameType):
    #print "numFoils: " + str(numFoils) #DEBUG
    #print "letterGroup: " + str(letterGroup) #DEBUG
    foilList = []
    foil_count = 0
    while foil_count < numFoils:
        l = random.choice(letterGroup)
        if (gameType == "Word" or gameType == "SoundWord"):
            if (not isTarget(l, targetList)):
                #print "add!" #DEBUG
                foilList.append(l)
                foil_count = foil_count + 1
        else:
            foilList.append(l)
            foil_count = foil_count + 1
        #print "foil_count is " + str(foil_count) #DEBUG
        #print "numFoils is " + str(numFoils) #DEBUG
    if type(targetList) == str:
        #print "blep" #DEBUG
        foilList.append(targetList)
    else:
        for t in targetList:
            foilList.append(t)
    return foilList
############################## MAIN ##########################################
groups =[]
numFoils = 0
with open('_levelGen_English.csv', 'rt') as csvfile:#need to setup path
    masterCSV = csv.reader(csvfile)
    next(masterCSV)
    for row in masterCSV:
        group = row[0]
        #print "group: " + str(group)
        messyLetters = row[1].split(',')
        letters = [x for x in messyLetters if x !=' ']
        letters = removeSpaces(letters)
        messySyllables = row[2].split(',')
        syllables = [x for x in messySyllables if x !=' ']
        syllables = removeSpaces(syllables)
        messyWords = row[3].split(',')
        words = [x for x in messyWords if x !=' ']
        words = removeSpaces(words)
        groups.append({"group":group, "letters": letters,
         "syllables": syllables, "words": words})
csvfile.close()

indir = 'xmlTemplates/test'
outdir = ''
oldg = -1
for root, dirs, filenames in os.walk (indir):
    for f in filenames:
        print ('\n' + f) #DEBUG
        tree = ET.parse (os.path.join(root,f))
        treeRoot = tree.getroot()
        inputType = treeRoot.get('monsterInputType')
        group = int(treeRoot.get('LettersGroup'))
        if (group > oldg):
            clearOldTargets()
            oldg = group

        if inputType == 'Letter':
            letterFill(group, treeRoot, False)
        elif inputType == 'SoundLetter':
            letterFill(group, treeRoot, True)
        elif (inputType == 'LetterInWord'):
            letterWordFill(group, treeRoot)
        elif inputType == 'Word':
            wordFill(group, treeRoot, False)
        else:
            wordFill(group, treeRoot, True)
        tree = ET.ElementTree(treeRoot)
        filename = outdir + f
        #rough_string = ET.tostring(treeRoot, 'utf-8')
        #reparsed = minidom.parseString(rough_string)
        #tree = reparsed.toprettyxml(indent="\t")
        tree.write(open(filename, 'wb'), encoding = 'utf-8')
        #file = open(filename, 'w')
        #file.write(tree)
        #file.close()
        tree = ""
        if(inputType == "SoundLetter"
        or inputType == "SoundWord"
        or inputType == "LetterInWord"):
            clearOldTargets()
