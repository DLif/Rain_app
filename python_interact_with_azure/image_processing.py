import colorsys
from PIL import Image
from scipy import ndimage


IMAGE_SIZE = 512
WHITE_RGB = (255, 255, 255)
WHITE_RGBA = (255, 255, 255, 255)
WHITE_TRANSPARENT = (255, 255, 255, 0)
BLACK = (0, 0, 0)
BLACK_TRANSPARENT = (0, 0, 0, 0)
COLOR_DIFF_THRESHOLD = 0.1 * 255
MAX_NUMBER_OF_DIFFERENT_PIXELS = 10


def color_diff(color_1, color_2):
    return sum(abs(col1 - col2) for col1, col2 in zip(color_1, color_2))


def get_color_diff(rgb_im_1, rgb_im_2):
    grids_and_borders = [[0 for dummy_row in range(IMAGE_SIZE)] for dummy_col in range(IMAGE_SIZE)]
    image_diff = [[0 for dummy_row in range(IMAGE_SIZE)] for dummy_col in range(IMAGE_SIZE)]
    for row in range(IMAGE_SIZE):
        for col in range(IMAGE_SIZE):
            if (col > 413 and row > 421) or (col < 22 and row < 141):  # border limits of logo and time
                continue
            elif BLACK == rgb_im_1.getpixel((row, col)):
                grids_and_borders[row][col] = 1
            elif color_diff(rgb_im_1.getpixel((row, col)), rgb_im_2.getpixel((row, col))) > COLOR_DIFF_THRESHOLD:
                image_diff[row][col] = 1
    return image_diff, grids_and_borders


def clean(img_diff):
    threshold = 0.5
    filtered_image = ndimage.filters.median_filter(img_diff, (3, 3))
    for row in range(IMAGE_SIZE):
        for col in range(IMAGE_SIZE):
            if filtered_image[row][col] < threshold:
                img_diff[row][col] = 0
    return img_diff


def get_non_black_rgb_values(rgb_im, mask):
    non_black_rgb_values = Image.new('RGB', (IMAGE_SIZE, IMAGE_SIZE), "white")
    for row in range(IMAGE_SIZE):
        for col in range(IMAGE_SIZE):
            if 1 == mask[row][col] and color_diff(rgb_im.getpixel((row, col)), BLACK) > COLOR_DIFF_THRESHOLD:
                non_black_rgb_values.putpixel((row, col), (rgb_im.getpixel((row, col))))
    return non_black_rgb_values


def get_neighbors(rgb_image, row, col):
    counter = 0
    x_range = range(max(0, row - 1), min(IMAGE_SIZE - 1, row + 1) + 1)
    y_range = range(max(0, col - 1), min(IMAGE_SIZE - 1, col + 1) + 1)
    num_neighbors = get_num_neighbors(x_range, y_range)
    neighbors = [[0 for dummy_row in range(3)] for dummy_col in range(num_neighbors)]
    for my_row in x_range:
        for my_col in y_range:
            if not (row == my_row and col == my_col):
                neighbors[counter] = rgb_image.getpixel((my_row, my_col))
                counter += 1
    return neighbors, num_neighbors


def get_num_neighbors(x_range, y_range):
    range_sum = len(x_range) + len(y_range)
    if range_sum == 4:
        return 3
    elif range_sum == 5:
        return 5
    elif range_sum == 6:
        return 8


def rgb2hsv(rgb_list):
    return [colorsys.rgb_to_hsv(color[0] / 255., color[1] / 255., color[2] / 255.) for color in rgb_list]


def hsv2rgb(hsv_list):
    rgb_list = [colorsys.hsv_to_rgb(hsv[0], hsv[1], hsv[2]) for hsv in hsv_list]
    return [(int(rgb[0] * 255), int(rgb[1] * 255), int(rgb[2] * 255)) for rgb in rgb_list]


def get_median_color(rgb_image, row, col):
    neighbors_rgb, num_neighbors = get_neighbors(rgb_image, row, col)
    neighbors_hsv = rgb2hsv(neighbors_rgb)
    neighbors_hsv.sort(key=lambda x: x[0])
    median_color_hsv = neighbors_hsv[int(num_neighbors / 2)]
    median_color_rgb = hsv2rgb([median_color_hsv])
    return median_color_rgb[0][0], median_color_rgb[0][1], median_color_rgb[0][2]


def fill_grids_and_borders(rain_overlay, grids_and_borders):
    for row in range(IMAGE_SIZE):
        for col in range(IMAGE_SIZE):
            if 1 == grids_and_borders[row][col]:
                rain_overlay.putpixel((row, col), (get_median_color(rain_overlay, row, col)))
    return rain_overlay


def white2alpha(rgb_image):
    img = rgb_image.convert("RGBA")
    data = img.getdata()
    new_data = []
    for pixel in data:
        if pixel == WHITE_RGBA:
            new_data.append(WHITE_TRANSPARENT)
        else:
            new_data.append(pixel)
    img.putdata(new_data)
    return img


def radar_off(gif_image,faliure_case):
    rgb_im = gif_image.convert('RGB')
    radar_off_img = Image.open(faliure_case).convert('RGB')

    img_diff, dummy = get_color_diff(rgb_im, radar_off_img)
    total_diff = sum(map(sum, img_diff))
    return total_diff < MAX_NUMBER_OF_DIFFERENT_PIXELS


def get_rain_overlay(gif_image):
    rgb_im = gif_image.convert('RGB')
    clean_img = Image.open("./clean_image.bmp")

    images_diff_mask, grids_and_borders = get_color_diff(rgb_im, clean_img)
    images_diff_mask = clean(images_diff_mask)
    rain_overlay = get_non_black_rgb_values(rgb_im, images_diff_mask)
    rain_overlay = fill_grids_and_borders(rain_overlay, grids_and_borders)
    return white2alpha(rain_overlay)

im = Image.open('input_image.gif')
im_c = get_rain_overlay(im)
im_c.save('test.png')
