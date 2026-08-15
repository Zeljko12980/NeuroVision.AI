interface ResponsiveImageProps {
    src: string;
}

export default function ResponsiveImage({ src }: ResponsiveImageProps) {
    return (
        <div className="relative">
            <div className="overflow-hidden">
                <img
                    src={src}
                    alt="Preview"
                    className="w-full max-h-40 object-cover border border-gray-200 rounded-xl dark:border-gray-800"
                />
            </div>
        </div>
    );
}