import pandas as pd
import numpy as np
import sys


def distance_3d(x1, y1, z1, x2, y2, z2):
    distance = np.linalg.norm(np.array([x2, y2, z2]) - np.array([x1, y1, z1]))
    return distance


def compute_distance(df: pd.DataFrame):
    distances = df.apply(lambda row: distance_3d(
        row["StrikeX"], row["StrikeY"], row["StrikeZ"],
        row["LycaonX"], row["LycaonY"], row["LycaonZ"]), axis=1)
    distances.name = "Distance"
    return pd.concat([distances, df["ElapsedTime"]], axis=1)


def get_indices(df: pd.DataFrame):
    last = 0
    indices = []
    time = df["ElapsedTime"]
    for d in enumerate(time):
        if d[1] < last:
            indices.append(d[0])
        last = d[1]
    # il faut les bords à chaque fois
    indices.insert(0, 0)
    indices.append(len(time))
    return indices


def get_party_number(index, indicies):
    for ind in indicies:
        if index < ind:
            return indicies.index(ind)


def separate_parties(df: pd.DataFrame):
    indices = get_indices(df)
    df["PartyNumber"] = df.apply(lambda row: get_party_number(row.name, indices), axis=1)
    return df


def compute_accuracy(file_name="accuracy.csv"):
    # Créer un DataFrame exemple
    df = pd.read_csv(file_name)

    # On calcule les distances
    df = compute_distance(df)

    # On obtient les différentes parties
    df = separate_parties(df)

    # on calcule la moyenne globale
    print("df:", df["Distance"].mean())

    # on calcule la moyenne par partie
    print("df:", df.groupby("PartyNumber")["Distance"].mean())

    # on calcule le temps maximal de chaque partie
    print("df:", df.groupby("PartyNumber")["ElapsedTime"].max())


if len(sys.argv) > 1:
    compute_accuracy(sys.argv[1])
