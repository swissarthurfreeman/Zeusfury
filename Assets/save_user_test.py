import os
import sys
import shutil


def move_files(folder_name):
    # Définition des noms de fichier
    file_names = ["accuracy.csv", "fitt.csv"]

    # Crée le nom du dossier de destination
    dest_folder = os.path.join("UserTests", folder_name)

    # Vérifie si le dossier de destination existe, sinon le crée
    if not os.path.exists(dest_folder):
        os.makedirs(dest_folder)

    # Déplace les fichiers
    for file_name in file_names:
        # Assurez-vous que le fichier existe avant de le déplacer
        if os.path.isfile(file_name):
            shutil.move(file_name, dest_folder)
        else:
            print(f"Le fichier {file_name} n'existe pas, donc il n'a pas été déplacé.")


move_files(sys.argv[1])
