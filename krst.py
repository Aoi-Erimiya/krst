#
# holst.py
#
# Copyright (c) 2019 Hiroaki Wada
#
# This software is released under the MIT License.
# http://opensource.org/licenses/mit-license.php
#
import random
from time import sleep

SLEEP_TIME = 0.5

class Player:
    def __init__(self, name, hand_cards):
        self.name = name
        self.hand_cards = hand_cards
        self.select_card = None
        self.ask_list = []

    def add_ask_list(self, ask_str):
        self.ask_list.push(ask_str)

class PlayerCharacter(Player):
    def choice_select_card(self):
        select_idx = int(input("何番目を選ぶ? -> ")) - 1
        self.select_card = self.hand_cards[select_idx]
        self.hand_cards.remove(self.select_card)

    def phase(self, target_player):
        select_mode = int(input("1:尋ねる 2:指名 -> "))
        if select_mode == 1:
            select_idx = int(input("どのカードで尋ねる? -> ")) -1
            ask_hololiver = choice(self.hand_cards, select_idx)
            sleep(SLEEP_TIME)
            if judge(target_player.select_card, ask_hololiver):
                target_player.ask_list.append(ask_hololiver.to_str() + "->T")
            else:
                target_player.ask_list.append(ask_hololiver.to_str() + "->F")
        else:
            ask_name = input("指名したい人の名前を入れてね -> ")
            ask(target_player.select_card, ask_name)

        print()
        show_asklist(target_player.ask_list)
        sleep(SLEEP_TIME)

class NonPlayerCharacter(Player):
    def choice_select_card(self):
        self.select_card = random.choice(self.hand_cards)
        self.hand_cards.remove(self.select_card)

    def phase(self, target_player):
        if len(self.hand_cards) == 0:
            return

        print("1:尋ねる 2:指名 -> 1")
        select_mode = 1
        if select_mode == 1:
            number = random.randint(0, len(self.hand_cards) - 1)
            print("どのカードで尋ねる? -> " + str(number))
            select_idx = number - 1
            ask_hololiver = choice(self.hand_cards, select_idx)
            sleep(SLEEP_TIME)
            if judge(target_player.select_card, ask_hololiver):
                target_player.ask_list.append(ask_hololiver.to_str() + "->T")
            else:
                target_player.ask_list.append(ask_hololiver.to_str() + "->F")
        else:
            ask_name = input("指名したい人の名前を入れてね -> ")
            ask(target_player.select_card, ask_name)

        print()
        show_asklist(target_player.ask_list)
        sleep(SLEEP_TIME)

class CycleList:
    def __init__(self, list):
        self.i = 0
        self.list = list

    def next(self):
        self.i = (self.i + 1) % len(self.list)
        return self.list[self.i]

    def previous(self):
        self.i = (self.i - 1 + len(self.list)) % len(self.list)
        return self.list[self.i]

    def present(self):
        return self.list[self.i]

    def reset(self):
        self.i = 0
        return

class Hololiver:
    def __init__(self, department, number, name):
        self.department = department
        self.number = number
        self.name = name

    def to_str(self):
        return self.department + "," + str(self.number) + "," + self.name

    def show(self):
        print(self.to_str())

def show_hands(hand_cards):
    print("*手札*")
    for i, card in enumerate(hand_cards):
        print(str(i + 1) + ":" + card.to_str())
    print("")

def show_asklist(asklist):
    print("*尋ねた履歴*")
    for s in asklist:
        print(s)
    print("")

def choice(hand_cards, idx):
    print("「この人、知ってますか？」")
    hand_cards[idx].show()
    print("")
    return hand_cards.pop(idx)

def judge(own_hololiver, ask_hololiver):
    if own_hololiver.number == ask_hololiver.number:
        print("「知ってます～」")
        return True

    if own_hololiver.department == ask_hololiver.department:
        print("「知ってます～」")
        return True

    print("「知りませんねぇ」")
    return False

def ask(own_hololiver, ask_hololiver_name):
    if own_hololiver.name == ask_hololiver_name:
        print("「そうです！」")
        return True

    print("「人違いですよ！」")
    return False


HOLOLIVERS = [
    Hololiver('ORG', 1, 'ときのそら'),
    Hololiver('ORG', 2, 'ロボ子さん'),
    Hololiver('ORG', 3, 'さくらみこ'),
    Hololiver('ORG', 4, 'YAGOO'),
    Hololiver('ORG', 5, 'FAMS'),
    Hololiver('ORG', 6, 'MOONグラデーション'),
    Hololiver('1st', 1, '夜空メル'),
    Hololiver('1st', 2, 'アキ・ローゼンタール'),
    Hololiver('1st', 3, '赤井はあと'),
    Hololiver('1st', 4, '白上フブキ'),
    Hololiver('1st', 5, '夏色まつり'),
    Hololiver('1st', 6, 'おるやんけ'),
    Hololiver('2nd', 1, '湊あくあ'),
    Hololiver('2nd', 2, '紫咲シオン'),
    Hololiver('2nd', 3, '百鬼あやめ'),
    Hololiver('2nd', 4, '癒月ちょこ'),
    Hololiver('2nd', 5, '大空スバル'),
    Hololiver('2nd', 6, 'スバルドダック'),
    Hololiver('3rd', 1, '兎田ぺこら'),
    Hololiver('3rd', 2, '潤羽るしあ'),
    Hololiver('3rd', 3, '不知火フレア'),
    Hololiver('3rd', 4, '白銀ノエル'),
    Hololiver('3rd', 5, '宝鐘マリン'),
    Hololiver('3rd', 6, 'ホロライブファンタジー'),
    Hololiver('OTR', 1, '大神ミオ'),
    Hololiver('OTR', 2, '戌神ころね'),
    Hololiver('OTR', 3, '猫又おかゆ'),
    Hololiver('OTR', 4, 'ゲーマーズ'),
    Hololiver('OTR', 5, 'Virtual Diva Azki'),
    Hololiver('OTR', 6, '星街すいせい')
]

random.shuffle(HOLOLIVERS)

player1 = PlayerCharacter("あなた", HOLOLIVERS[0:6])
player2 = NonPlayerCharacter("NPC1", HOLOLIVERS[7:13])
player3 = NonPlayerCharacter("NPC2", HOLOLIVERS[14:20])
player4 = NonPlayerCharacter("NPC3", HOLOLIVERS[21:27])

players = CycleList([player1, player2, player3, player4])

show_hands(player1.hand_cards)

for player in players.list:
    player.choice_select_card()

print("---------")
print("*選択したのはこの人です*")
player1.select_card.show()
print("---------")
print()

while(True):
    print("---------")
    print("次のたーん")
    print("---------")
    show_hands(player1.hand_cards)
    print()
    print("自分のもの")
    show_asklist(player1.ask_list)
    print("相手プレイヤー")
    show_asklist(player2.ask_list)
    print("他プレイヤー")
    show_asklist(player3.ask_list)
    show_asklist(player4.ask_list)

    sleep(SLEEP_TIME)
    for player in players.list:
        print("---------\n" + player.name + "の番です\n---------")
        player.phase(players.next())
        sleep(SLEEP_TIME)