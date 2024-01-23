from PIL import Image
import sys

def view_image(file_path, save_png_path=None):
    try:
        image = Image.open(file_path)
    except IOError:
        print("Error: Cannot open image.")
        sys.exit(1)

    if save_png_path:
        image.save(save_png_path)
    
    image.show()

if __name__ == "__main__":
    if len(sys.argv) == 2:
        file_path = sys.argv[1]
        view_image(file_path)
    elif len(sys.argv) == 3:
        file_path = sys.argv[1]
        save_png_path = sys.argv[2]
        view_image(file_path, save_png_path)
    else:
        print("Usage: python viewer.py <file_path> <save_png_path>(optional)")
